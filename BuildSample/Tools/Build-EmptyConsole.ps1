$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

$slnFilePath = "..\BuildSample.sln"
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Clean", $msbuild, $slnFilePath))
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Rebuild", $msbuild, $slnFilePath))

cd ..\EmptyConsole
xcopy Data bin\Release\Data /D/E/C/I/H/Y

# In case of PowerShell, thw work dir is that of .ps1 file.
..\Tools\CreateZipForAssembly.ps1 ..\EmptyConsole\bin\Release\EmptyConsole.exe ..\Downloads

explorer ..\Downloads
