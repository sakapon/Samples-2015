$slnFilePath = "..\BuildSample.sln"
$targetDirPath = "..\Downloads"

$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Clean", $msbuild, $slnFilePath))
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Rebuild", $msbuild, $slnFilePath))

explorer $targetDirPath
