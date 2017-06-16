$coverJsonFile = "cover.json"
$codeCoverageFile = "CodeCoverage.runsettings"
$coverageXmlFile = "coverage.coveragexml"

$xunitRunnerVSPath = (Resolve-Path ".\packages\xunit.runner.visualstudio.*\build\_common\").Path
$unitTestsPath = ".\tests\Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests\bin\Release\Serilog.Sinks.AzureWebJobsTraceWriter.UnitTests.dll"

$vsCodeCoverageExe = "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe"

vstest.console.exe /inIsolation /Enablecodecoverage /TestAdapterPath:"$xunitRunnerVSPath" "$unitTestsPath" /Settings:$codeCoverageFile

$coverageFilePath = (Resolve-Path -path "TestResults\*\*.coverage").Path

if (Test-Path -Path ".\$coverageXmlFile")
{
     Remove-Item -Path ".\$coverageXmlFile"
}

.$vsCodeCoverageExe analyze /output:$coverageXmlFile "$coverageFilePath"

Push-AppveyorArtifact "$coverageXmlFile"

$coveralls = (Resolve-Path "src/packages/coveralls.net.*/tools/csmacnz.coveralls.exe").Path

& $coveralls --dynamiccodecoverage -i coverage.coveragexml --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID --useRelativePaths -o $coverJsonFile

Push-AppveyorArtifact "$coverJsonFile"