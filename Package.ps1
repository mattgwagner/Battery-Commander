param (
	[string]$Configuration = 'Debug'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$DotNet = "${env:ProgramFiles}\dotnet\dotnet.exe"

$Website = Join-Path $Here "Battery-Commander.Web\Battery-Commander.Web.csproj"

& $DotNet publish $Website --configuration $Configuration --output "$Here/output"

7z a BatteryCommander.Web.zip "$Here/output"

EXIT $LASTEXITCODE