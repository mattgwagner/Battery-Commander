param (
	[string]$Configuration = 'Debug'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$7Zip = "${env:ProgramFiles}\7-Zip\7z.exe"

$DotNet = "${env:ProgramFiles}\dotnet\dotnet.exe"

$Website = Join-Path $Here "Battery-Commander.Web\Battery-Commander.Web.csproj"

& $DotNet publish $Website --configuration $Configuration --output "$Here/output"

& $7Zip a BatteryCommander.Web.zip "$Here/output/**"

EXIT $LASTEXITCODE