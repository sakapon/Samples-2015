[System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem")

$parent = [System.IO.Directory]::GetParent($Args[1])
[System.IO.Directory]::CreateDirectory($parent)
[System.IO.File]::Delete($Args[1])
[System.IO.Compression.ZipFile]::CreateFromDirectory($Args[0], $Args[1])
