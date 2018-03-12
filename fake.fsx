#r @"./tools/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.EnvironmentHelper
open Fake.StringHelper
open System
open System.IO
// open System.Management.Automation

let buildDir = "./build"
let slnFilePath = "./src/EnergyProviderAdapter.sln"
let version = (ReadLine "./VERSION") + "." + (environVarOrDefault "GO_PIPELINE_COUNTER" "0")
let testOutputFilePath = Path.Combine(buildDir, "testoutput.log")




Target "Clean" (fun _ ->
   CleanDir buildDir
)

Target "RestoreNuGetPackages" (fun _ ->
   let result =
      ExecProcess (fun info -> 
         info.FileName <- "./tools/NuGet/nuget.exe"
         info.Arguments <- "restore " + slnFilePath + " -noninteractive" 
      )(System.TimeSpan.FromMinutes 2.0)     
   
   if result <> 0 then failwith "NuGet restore failed or timed out"  
)

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

let runTests testAssemblySearchPath =
   let args = (!! testAssemblySearchPath) |> String.concat " "
   let result =
      ExecProcess (fun info ->
         info.FileName <- "./tools/NUnit.Console/nunit3-console.exe"
         info.Arguments <- String.concat " " [args; "/framework=net-4.5"; "--trace=off"; "--out=" + testOutputFilePath]
      )(System.TimeSpan.FromMinutes 4.0)

   trace "========================================================="

   trace testOutputFilePath

   trace "========================================================="

   if result <> 0 then failwith "Tests failed or timed out"

Target "RunUnitTests" (fun _ ->
   trace "Running Unit Tests..."
   let outputPath = currentDirectory @@ buildDir @@ "Release" @@ "/*LibTests*.dll"
   runTests outputPath
)

let compileSingleProviderAdapter providerName brandCodePrefix =
   let outputPath = currentDirectory @@ buildDir @@ "Release" 
      
   let compileOptions defaults =
      { 
         defaults with
            Verbosity = Some MSBuildVerbosity.Minimal
            Targets = ["Build"]
            RestorePackagesFlag = true
            Properties =
            [
               "OutputPath", outputPath
            ]
      }
   
   build compileOptions slnFilePath |> DoNothing

Target "Compile" (fun _ ->
    compileSingleProviderAdapter "EnergyHelpLine" "EHL"
)   

Target "Package" (fun _ ->
   trace "Packaging..."
   let packagedDir = "./build-packaged"
   CreateDir packagedDir
   
   let packagedFilePath = Path.Combine(packagedDir, "all.zip")
   
   !! (buildDir + "/**/*.*")
     -- "/**/*.pdb"
     |> Zip buildDir (packagedFilePath)

   printfn "##teamcity[publishArtifacts '%s']" packagedFilePath
)

Target "prepush" (fun _ ->
   trace "========================================================="
   trace "Finished running all tests successfully. OK TO PUSH"
   trace "========================================================="
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


"Clean"
   ==> "RestoreNuGetPackages"
   ==> "GenerateAssemblyInfo"
   ==> "Compile"
   ==> "RunUnitTests"
   ==> "prepush"
   ==> "Package"

RunTargetOrDefault "Package"
