properties {
	$base_directory = Resolve-Path .
	$src_directory = "$base_directory\src\IdentityModel.HttpSigning"
	$output_directory = "$base_directory\build"
	$dist_directory = "$base_directory\distribution"
	$sln_file = "$base_directory\IdentityModel.HttpSigning.sln"
	$target_config = "Release"
	$nuget_path = "$base_directory\packages\nuget.exe"
	$xunit_path = "$base_directory\packages\xunit.runner.console\tools\xunit.console.exe"

	$buildNumber = 0;
	$version = "1.0.0.0"
	$preRelease = $null
}

task default -depends Clean, RunTests, CreateNuGetPackage
task appVeyor -depends Clean, CreateNuGetPackage

task Clean {
	rmdir $output_directory -ea SilentlyContinue -recurse
	rmdir $dist_directory -ea SilentlyContinue -recurse
	exec { msbuild /nologo /verbosity:quiet $sln_file /p:Configuration=$target_config /t:Clean }
}

task Compile -depends UpdateVersion {
  exec { & "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" /nologo /verbosity:q $sln_file /p:Configuration=$target_config /p:TargetFrameworkVersion=v4.5 }
}

task UpdateVersion {
	$vSplit = $version.Split('.')
	if($vSplit.Length -ne 4)
	{
		throw "Version number is invalid. Must be in the form of 0.0.0.0"
	}
	$major = $vSplit[0]
	$minor = $vSplit[1]
	$patch = $vSplit[2]
	$assemblyFileVersion =  "$major.$minor.$patch.$buildNumber"
	$assemblyVersion = "$major.$minor.0.0"
	$versionAssemblyInfoFile = "$src_directory/VersionAssemblyInfo.cs"
	"using System.Reflection;" > $versionAssemblyInfoFile
	"" >> $versionAssemblyInfoFile
	"[assembly: AssemblyVersion(""$assemblyVersion"")]" >> $versionAssemblyInfoFile
	"[assembly: AssemblyFileVersion(""$assemblyFileVersion"")]" >> $versionAssemblyInfoFile
}

task RunTests -depends Compile {
	$project = "IdentityModel.HttpSigning.Tests"
	mkdir $output_directory\xunit\$project -ea SilentlyContinue
	.$xunit_path "$output_directory\$project.dll"
}

task CreateNuGetPackage -depends Compile {
	$vSplit = $version.Split('.')
	if($vSplit.Length -ne 4)
	{
		throw "Version number is invalid. Must be in the form of 0.0.0.0"
	}
	$major = $vSplit[0]
	$minor = $vSplit[1]
	$patch = $vSplit[2]
	$packageVersion =  "$major.$minor.$patch"
	if($preRelease){
		$packageVersion = "$packageVersion-$preRelease"
	}

	if ($buildNumber -ne 0){
		$packageVersion = $packageVersion + "-build" + $buildNumber.ToString().PadLeft(5,'0')
	}

	new-item $dist_directory -type directory

	copy-item $base_directory\IdentityModel.HttpSigning.nuspec $dist_directory

	new-item $dist_directory\lib\net45\ -type directory
	copy-item $output_directory\IdentityModel.HttpSigning.dll $dist_directory\lib\net45\
	copy-item $output_directory\IdentityModel.HttpSigning.pdb $dist_directory\lib\net45\

	exec { . $nuget_path pack $dist_directory\IdentityModel.HttpSigning.nuspec -BasePath $dist_directory -o $dist_directory -version $packageVersion }
}
