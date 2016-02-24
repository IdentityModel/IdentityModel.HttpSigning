Param(
	[string]$buildNumber = "0",
	[string]$preRelease = $null
)

$packages_exists = Test-Path packages
if ($packages_exists -eq $false) {
    mkdir packages
}

$exists = Test-Path packages\nuget.exe
if ($exists -eq $false) {
    $source = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    Invoke-WebRequest $source -OutFile packages\nuget.exe
}

.\packages\nuget.exe install psake -ExcludeVersion -o packages -nocache
.\packages\nuget.exe install xunit.runner.console -ExcludeVersion -o packages -nocache

gci .\src -Recurse "packages.config" |% {
	"Restoring " + $_.FullName
	.\packages\nuget.exe install $_.FullName -o .\packages
}

Import-Module .\packages\psake\tools\psake.psm1

if(Test-Path Env:\APPVEYOR_BUILD_NUMBER){
	$buildNumber = [int]$Env:APPVEYOR_BUILD_NUMBER
	Write-Host "Using APPVEYOR_BUILD_NUMBER"

	$task = "appVeyor"
}

"Build number $buildNumber"

Invoke-Psake .\default.ps1 $task -framework "4.0x64" -properties @{ buildNumber=$buildNumber; preRelease=$preRelease }

Remove-Module psake