$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

cd ..
Invoke-Expression ($msbuild + " BuildSample.sln /p:Configuration=Release /t:Clean")
Invoke-Expression ($msbuild + " BuildSample.sln /p:Configuration=Release /t:Rebuild")

explorer Downloads
