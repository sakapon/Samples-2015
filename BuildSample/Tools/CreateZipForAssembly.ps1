# source assembly file path
if (-not ($Args[0])) { return 100 }
# target dir path
if (-not ($Args[1])) { return 101 }

[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")

$assemblyName = [System.IO.Path]::GetFileNameWithoutExtension($Args[0])
$assembly = [System.Reflection.Assembly]::LoadFrom($Args[0])
$assemblyFileVersion = [System.Reflection.CustomAttributeExtensions]::GetCustomAttribute($assembly, [System.Reflection.AssemblyFileVersionAttribute])
if (-not ($assemblyFileVersion)) { return 200 }

$sourceDirPath = [System.IO.Path]::GetDirectoryName($Args[0])
$targetZipFileName = [string]::Format("{0}-{1}.zip", $assemblyName, $assemblyFileVersion.Version)
$targetZipFilePath = [System.IO.Path]::Combine($Args[1], $targetZipFileName)

[System.IO.Directory]::CreateDirectory($Args[1])
[System.IO.File]::Delete($targetZipFilePath)
[System.IO.Compression.ZipFile]::CreateFromDirectory($sourceDirPath, $targetZipFilePath)
