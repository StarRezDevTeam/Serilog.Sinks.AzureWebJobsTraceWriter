$codeCoverageFile = "CodeCoverage.runsettings"
$coverageXmlFile = "TestResults\coverage.coveragexml"

$xunitRunnerVSPath = (Resolve-Path ".\packages\xunit.runner.visualstudio.*\build\_common\").Path
$coverallsPath = (Resolve-Path ".\packages\coveralls.net.*\tools\csmacnz.coveralls.exe").Path

$unitTestsPath = ".\tests\Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests\bin\Release\Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests.dll"

$vsCodeCoverageExe = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe"

if (!(Test-Path -Path $vsCodeCoverageExe))
{
    $vsCodeCoverageExe = "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe"
}

# Cleanup before we start

if (Test-Path -Path "TestResults\")
{
     Remove-Item -Path "TestResults\" -Recurse -Force
}
if (Test-Path -Path ".\$coverageXmlFile")
{
     Remove-Item -Path ".\$coverageXmlFile"
}
if (Test-Path -Path ".\$coverageXmlFile")
{
     Remove-Item -Path ".\$coverageXmlFile"
}

vstest.console.exe /inIsolation /Enablecodecoverage /TestAdapterPath:"$xunitRunnerVSPath" /Settings:$codeCoverageFile /logger:Appveyor "$unitTestsPath" 

$coverageFilePath = (Resolve-Path -path "TestResults\*\*.coverage").Path

if (Test-Path -Path $coverageXmlFile)
{
     Remove-Item -Path $coverageXmlFile
}

& $vsCodeCoverageExe analyze /output:$coverageXmlFile "$coverageFilePath"

& $coverallsPath --dynamiccodecoverage -i $coverageXmlFile --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID --useRelativePaths