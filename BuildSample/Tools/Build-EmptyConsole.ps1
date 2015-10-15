$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

.\IncrementVersion-cs.ps1 ..\EmptyConsole

$slnFilePath = "..\BuildSample.sln"
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Clean", $msbuild, $slnFilePath))
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Rebuild", $msbuild, $slnFilePath))

# In case of PowerShell, the work directory is that of .ps1 file.
.\CreateZipForAssembly.ps1 ..\EmptyConsole\bin\Release\EmptyConsole.exe ..\Downloads

explorer ..\Downloads
