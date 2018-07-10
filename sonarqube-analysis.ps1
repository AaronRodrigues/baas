$ErrorActionPreference = "Stop"

write-output "=================================================="
write-output "   Starting SonarQube runner"
write-output "=================================================="
& .\Tools\SonarQube-Scanner-for-MSbuild\MSBuild.SonarQube.Runner.exe begin /key:"EnergyProviderAdapter" /name:"Energy Provider Adapter" /version:"1.0" /d:sonar.cs.opencover.reportsPaths="opencover.xml"
if ($LastExitCode -ne 0) {
	throw 'Failed to start SonarQube runner'
}

write-output "=================================================="
write-output "   Running fake"
write-output "=================================================="
& .\fake.bat mode=Debug target=AnalyseTestCoverage
if ($LastExitCode -ne 0) {
	throw 'Failed to run fake'
}

write-output "=================================================="
write-output "   Stopping SonarQube runner and submitting results"
write-output "=================================================="
& .\Tools\SonarQube-Scanner-for-MSbuild\MSBuild.SonarQube.Runner.exe end
if ($LastExitCode -ne 0) {
	throw 'Failed to stop SonarQube runner or submitting results'
}

write-output "=================================================="
write-output "   Verifying that build passed all quality gates"
write-output "=================================================="
$statusUrl = "https://sonarqube-security.test.ctmers.io//api/qualitygates/project_status?projectKey=EnergyProviderAdapter"
$statusResponse = $statusWebrequest = Invoke-WebRequest -Uri $statusUrl -Method GET
$status = convertfrom-json $statusResponse
if ($status.projectStatus.status -ne "OK") {	
	write-host $statusResponse
	throw 'Build failed to pass one or more SonarQube quality gates. See the dashboard for more info.'
}

write-host "All quality gates passed \o/"
