﻿$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

$slnFilePath = "..\BuildSample.sln"
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Clean", $msbuild, $slnFilePath))
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Rebuild", $msbuild, $slnFilePath))

explorer ..\Downloads
