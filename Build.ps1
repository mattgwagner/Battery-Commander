param (
	## The build configuration, i.e. Debug/Release
	[string]$Configuration = 'Debug'
)

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$SolutionFile = Join-Path $Here "Battery-Commander.sln"

$DotNet = "${env:ProgramFiles}\dotnet\dotnet.exe"

& $DotNet restore -v Minimal

& $DotNet build $SolutionFile --configuration $Configuration

EXIT $LASTEXITCODE