$ErrorActionPreference = 'Stop'

Set-Location -LiteralPath $PSScriptRoot

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

try {
    dotnet cake build.cake --target=Version --verbosity=Diagnostic
}
catch {
    Write-Host "GitVersion / Version step failed:"
    Write-Host $_.Exception.ToString()
    throw
}