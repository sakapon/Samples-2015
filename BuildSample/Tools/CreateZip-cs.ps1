# source directory path
if (-not ($Args[0])) { return 100 }
# target zip file path
if (-not ($Args[1])) { return 101 }

$references = @("System.IO.Compression.FileSystem")
$source = @"
using System;
using System.IO;
using System.IO.Compression;

public static class ZipHelper
{
    public static void CreateZip(string sourceDirPath, string targetZipFilePath)
    {
        var targetDirPath = Path.GetDirectoryName(targetZipFilePath);
        Directory.CreateDirectory(targetDirPath);
        File.Delete(targetZipFilePath);
        ZipFile.CreateFromDirectory(sourceDirPath, targetZipFilePath);
    }
}
"@

Add-Type -TypeDefinition $source -Language CSharp -ReferencedAssemblies $references
[ZipHelper]::CreateZip($Args[0], $Args[1])
