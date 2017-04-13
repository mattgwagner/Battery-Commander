param (
	[string]$Configuration = 'Debug'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$DotNet = "${env:ProgramFiles}\dotnet\dotnet.exe"

$Tests = Join-Path $Here "Battery-Commander.Web\Battery-Commander.Web.csproj"

& $DotNet pack $SolutionFile --configuration $Configuration

EXIT $LASTEXITCODE