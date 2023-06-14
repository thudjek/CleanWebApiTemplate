Set-StrictMode -Version Latest

$projectName = "CleanWebApiTemplate"
$rootOutputPath = "C:/NugetPackages/$($projectName)_Nuget"
$copyToPath = "$rootOutputPath/content/$projectName"
$nugetExePath = "$rootOutputPath/nuget.exe"
$nugetOutputPath =  "$rootOutputPath/nupkg"
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"


Write-Output "Copy template content"
Copy-Item -Path "./" -Recurse -Destination "$copyToPath" -Container

if(-not(Test-Path -Path $nugetExePath -PathType Leaf))
{
	Write-Output "Download nuget.exe from $nugetUrl"
	Invoke-WebRequest -Uri $nugetUrl -OutFile $nugetExePath
}
else
{
	Write-Output "nuget.exe exists, no need to download"
}

Write-Output "Pack nuget"
$cmdArgList = @( "pack", "$copyToPath\CleanWebApiTemplate.nuspec",
				 "-OutputDirectory", "$nugetOutputPath", "-NoDefaultExcludes")
				 
& $nugetExePath $cmdArgList