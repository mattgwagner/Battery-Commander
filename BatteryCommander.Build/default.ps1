$PSake.use_exit_on_error = $true

if(!$Configuration) { $Configuration = "Debug" }

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$SolutionRoot = (Split-Path -parent $Here)

Import-Module "$Here\Common" -DisableNameChecking

$SolutionFile = "$SolutionRoot\BatteryCommander.sln"

## $ToolsDir = Join-Path $SolutionRoot "lib"

$NuGet = Join-Path $SolutionRoot ".nuget\nuget.exe"

$MSBuild ="${env:ProgramFiles(x86)}\MSBuild\12.0\Bin\msbuild.exe"

$ConnectionString = "Data Source=localhost;Initial Catalog=BatteryCommander;Integrated Security=SSPI;"

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task default -depends Build

Task Build -depends Restore-Packages {
	exec { . $MSBuild $SolutionFile /t:Build /v:minimal /p:Configuration=$Configuration }
}

Task Clean {
	Remove-Item -Path "$SolutionRoot\packages\*" -Exclude repositories.config -Recurse -Force 
	Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { Remove-Item $_.fullname -Force -Recurse }
	exec { . $MSBuild $SolutionFile /t:Clean /v:quiet }
}

Task Push-Dev -depends Build {
	Push-Code "Dev"
}

Task Push-Prod -depends Build {
	Push-Code "Prod"
}

function Push-Code($Environment)
{
	robocopy "$SolutionRoot\BatteryCommander.Web" "\\app-server\Apps\BatteryCommander.$Environment" /mir
}

Task Restore-Packages -depends Install-BuildTools {
	exec { . $NuGet restore $SolutionFile }
}

Task Install-MSBuild {
    if(!(Test-Path "${env:ProgramFiles(x86)}\MSBuild\12.0\Bin\msbuild.exe")) 
	{ 
		cinst microsoft-build-tools
	}
}

Task Install-Win8SDK {
    if(!(Test-Path "${env:ProgramFiles(x86)}\Windows Kits\8.1\bin\x64\signtool.exe"))
	{ 
		cinst windows-sdk-8.1 
	}
}

Task Install-WebAppTargets {
    if(!(Test-Path "$env:ChocolateyInstall\lib\MSBuild.Microsoft.VisualStudio.Web.targets.12.0.4\tools\VSToolsPath\WebApplications\Microsoft.WebApplication.targets"))
	{ 
        cinst MSBuild.Microsoft.VisualStudio.Web.targets -source http://packages.nuget.org/v1/FeedService.svc/ -version '12.0.4'
    }
}

Task Install-BuildTools -depends Install-MSBuild, Install-Win8SDK, Install-WebAppTargets

# Borrowed from Luis Rocha's Blog (http://www.luisrocha.net/2009/11/setting-assembly-version-with-windows.html)
Task Update-AssemblyInfoFiles {
	$assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileCommitPattern = 'AssemblyInformationalVersion\("(.*?)"\)'

    $assemblyVersion = 'AssemblyVersion("' + $Version + '")';
    $fileVersion = 'AssemblyFileVersion("' + $Version + '")';
    $commitVersion = 'AssemblyInformationalVersion("' + $InformationalVersion + '")';

    Get-ChildItem -path $SolutionRoot -r -filter GlobalAssemblyInfo.cs | ForEach-Object {
        $filename = $_.Directory.ToString() + '\' + $_.Name
        $filename + ' -> ' + $Version
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion } |
            % {$_ -replace $fileCommitPattern, $commitVersion }
        } | Set-Content $filename
    }
}