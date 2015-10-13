# source directory path
if (-not ($Args[0])) { return 100 }
# target zip file path
if (-not ($Args[1])) { return 101 }

Add-Type -AssemblyName System.IO.Compression.FileSystem

$parent = [System.IO.Path]::GetDirectoryName($Args[1])
[System.IO.Directory]::CreateDirectory($parent)
[System.IO.File]::Delete($Args[1])
[System.IO.Compression.ZipFile]::CreateFromDirectory($Args[0], $Args[1])
