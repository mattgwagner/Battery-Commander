### 
### Common functions module
###

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Definition

$SolutionRoot = Split-Path -Parent $ScriptPath

Export-ModuleMember -Variable @('ScriptPath', 'SolutionRoot')

function Install-Chocolatey()
{
	if(-not $env:ChocolateyInstall -or -not (Test-Path "$env:ChocolateyInstall"))
	{
		Write-Output "Chocolatey Not Found, Installing..."
		iex ((new-object net.webclient).DownloadString('http://chocolatey.org/install.ps1')) 
	}
}

function Install-Psake()
{
	if(!(Test-Path $env:ChocolateyInstall\lib\Psake*)) 
	{ 
		choco install psake 
	}
}

Export-ModuleMember -Function *-*