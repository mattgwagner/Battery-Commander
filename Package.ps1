param (
	[string]$Configuration = 'Debug'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$DotNet = "${env:ProgramFiles}\dotnet\dotnet.exe"

$Website = Join-Path $Here "Battery-Commander.Web\Battery-Commander.Web.csproj"

& $DotNet pack $Website --configuration $Configuration

EXIT $LASTEXITCODE