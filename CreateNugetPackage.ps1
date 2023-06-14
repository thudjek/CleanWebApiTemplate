Set-StrictMode -Version Latest

$templateFolderName = "CleanWebApiTemplate_Nuget"
$rootOutputPath = "C:/NugetPackages/$templateFolderName"
$contentPath = "$rootOutputPath/content"
$nugetExePath = "$rootOutputPath/nuget.exe"
$nugetOutputPath =  "$rootOutputPath/nupkg"
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/v5.9.1/nuget.exe"


Write-Output "Copy template"
Copy-Item -Path "./" -Recurse -Destination "$contentPath/CleanWebApiTemplate" -Container

Write-Output "Copy nuspec"
Copy-item -Force -Recurse "CleanWebApiTemplate.nuspec" -Destination $rootOutputPath

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
$cmdArgList = @( "pack", "$rootOutputPath\CleanWebApiTemplate.nuspec",
				 "-OutputDirectory", "$nugetOutputPath", "-NoDefaultExcludes")
				 
& $nugetExePath $cmdArgList