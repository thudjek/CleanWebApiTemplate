Set-StrictMode -Version Latest

$templateName = "template"
$templatePath =     "./$templateName/mtr"
$contentDirectory = "./$templateName/mtr/content"
$nugetExePath = "./$templateName/nuget.exe"
$nugetOut =  "./$templateName/nupkg"
$nugetUrl = "https://dist.nuget.org/win-x86-commandline/v5.9.1/nuget.exe"


Write-Output "Copy template"
Copy-Item -Path "./src/CleanWebApiTemplate" -Recurse -Destination "$contentDirectory/CleanWebApiTemplate" -Container

Write-Output "Copy nuspec"
Copy-item -Force -Recurse "CleanWebApiTemplate.nuspec" -Destination $templatePath

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
$cmdArgList = @( "pack", "$templatePath\CleanWebApiTemplate.nuspec",
				 "-OutputDirectory", "$nugetOut", "-NoDefaultExcludes")
				 
& $nugetExePath $cmdArgList