#r "./tools/FAKE.4.61.2/tools/FakeLib.dll" 

open Fake
open Fake.Testing.NUnit3
open Fake.AssemblyInfoFile
open System.IO

let version = (ReadLine "./VERSION") + "." + (environVarOrDefault "GO_PIPELINE_COUNTER" "0")

let mode = getBuildParamOrDefault "mode" "Release"
printfn "Mode is: %s" mode

let restoreNugetPackages() =
   let result =
      ExecProcess (fun info ->
         info.FileName <- "./tools/NuGet/nuget.exe"
         info.Arguments <- "restore ./src/EnergyProviderAdapter.sln -noninteractive -configfile ./src/nuget.config"
      )(System.TimeSpan.FromMinutes 5.0)

   if result <> 0 then failwith "NuGet restore failed or timed out"

let compile() =
    !! (sprintf "./src/**/bin/%s/" mode) |> CleanDirs
    
    build (fun x -> 
        { x with Verbosity = Some MSBuildVerbosity.Quiet
                 RestorePackagesFlag = true
                 NodeReuse = false
                 Properties = [ "Configuration", mode ] }) "./src/EnergyProviderAdapter.sln" 

let runUnitTests() =
    let tests = [ 
        (sprintf "./src/Energy.EHLCommsLibTests/bin/%s/Energy.EHLCommsLibTests.dll" mode);
        (sprintf "./src/Energy.ProviderAdapterTests/bin/%s/Energy.ProviderAdapterTests.dll" mode);
    ]

    let nUnitParams _ = 
        { 
            NUnit3Defaults with 
                TimeOut = System.TimeSpan.FromMinutes(4.0)
                //TraceLevel = NUnit3TraceLevel.Off
                OutputDir = "./nunit-output.log"
        }
    tests |> NUnit3 nUnitParams


let runMemoryProfileTests() =
    let result =
        ExecProcess (fun info ->
             info.FileName <- ( sprintf "./src/Energy.ProviderAdapterMemoryProfiling/bin/%s/Energy.ProviderAdapterMemoryProfiling.exe" mode)
        )(System.TimeSpan.FromMinutes 5.0)

    if result <> 0 then failwith "Memory Profiling Tests failed or timed out" 

Target "GenerateAssemblyInfo" (fun _ ->
   let now = System.DateTime.Now
   let tcBuildNumber = environVarOrDefault "GO_PIPELINE_COUNTER" "0"
   let revision = if tcBuildNumber <> "" then tcBuildNumber else Git.Information.getCurrentSHA1("./")
   let info = System.String.Format("Built on {0} at {1} from revision {2}",
                                    now.ToShortDateString(),
                                    now.ToLongTimeString(),
                                    revision)
   let assemblyAttributes =
      [
         Attribute.Version version
         Attribute.FileVersion version
         Attribute.InformationalVersion version
         Attribute.Configuration info
      ]
   CreateCSharpAssemblyInfo "./src/Version/AssemblyInfo.cs" assemblyAttributes
)
  
Target "Package" (fun _ ->
   trace "Packaging..."
   let packagedDir = "./build-packaged"
   CreateDir packagedDir
   
   let buildDir = (sprintf "./src/Energy.ProviderAdapter/bin/%s/" mode)
   let packagedFilePath = Path.Combine(packagedDir, "all.zip")
   
   !! (buildDir + "/**/*.*")
     -- "/**/*.pdb"
     |> Zip buildDir (packagedFilePath)
)

Target "PrePush" (fun _ ->
   trace "========================================================="
   trace "Finished running all tests successfully. OK TO PUSH"
   trace "========================================================="
)

Target "AnalyseTestCoverage" (fun _ ->

    let nunitArgs = [
                        (sprintf "./src/Energy.EHLCommsLibTests/bin/%s/Energy.EHLCommsLibTests.dll" mode) 
                        (sprintf "./src/Energy.ProviderAdapterTests/bin/%s/Energy.ProviderAdapterTests.dll" mode) 

                        "--trace=Off";
                        "--output=./nunit-output.log";

                        // Ignore performance tests when calculating coverage
                        "--where=cat!=performance"

                    ] |> String.concat " "
    let allArgs = [ 
                    "-register:path64"; 
                    "-output:\"opencover.xml\"";
                    "-returntargetcode:1";
                    "-hideskipped:All";
                    "-skipautoprops";
                    
                    // Workaround for issue where opencover tries to cover some dependencies
                    "-filter:\"+[*]* -[*Tests]* -[Moq*]* -[nunit.framework*]*\""; 

                    "-target:\"./tools/NUnit.Console/nunit3-console.exe\"";
                    sprintf "-targetargs:\"%s\"" nunitArgs
                  ]
    let result = 
        ExecProcess (fun info ->
            info.FileName <- "./Tools/OpenCover/tools/OpenCover.Console.exe"
            info.Arguments <- allArgs |> String.concat " "
        )(System.TimeSpan.FromMinutes 7.0)

    if result <> 0 then failwith "Test coverage via OpenCover failed or timed-out"
)

Target "CreateTestCoverageReport" (fun _ ->
    let args = [
                    "-reports:./opencover.xml";
                    "-verbosity:Warning";
                    "-targetdir:./coverage-report";
               ]
    let result = 
        ExecProcess (fun info ->
            info.FileName <- "./Tools/ReportGenerator/tools/ReportGenerator.exe"
            info.Arguments <- args |> String.concat " "
        )(System.TimeSpan.FromMinutes 7.0)

    if result <> 0 then failwith "Test coverage via OpenCover failed or timed-out"
)

Target "Deploy" (fun _ ->
  let env = environVarOrFail "DEPLOY_ENV"

  let script = sprintf "/c powershell -ExecutionPolicy Unrestricted ./deploy.ps1 -env %s" env

  let p = new System.Diagnostics.Process();              
  p.StartInfo.FileName <- "cmd.exe";
  p.StartInfo.Arguments <- (script)
  p.StartInfo.RedirectStandardError <- true
  p.StartInfo.RedirectStandardOutput <- true
  p.StartInfo.UseShellExecute <- false
  p.Start() |> ignore

  let err = p.StandardError.ReadToEnd() 

  if err.Length <> 0 then 
    printfn "%A" (err)
    failwith "deploy failed"
  else  
    printfn "Processing..."
    printfn "%A" (p.StandardOutput.ReadToEnd())
    printfn "Finished"
)

Target "RestoreNuGetPackages" restoreNugetPackages
Target "Compile" compile
Target "RunUnitTests" runUnitTests 
Target "RunMemoryProfileTests" runMemoryProfileTests

"RestoreNuGetPackages"
   ==> "GenerateAssemblyInfo"
   ==> "Compile"
   ==> "RunUnitTests"
   ==> "RunMemoryProfileTests"
   ==> "PrePush"
   ==> "Package"

"Compile" ==> "AnalyseTestCoverage" ==> "CreateTestCoverageReport"

RunTargetOrDefault "Package"
