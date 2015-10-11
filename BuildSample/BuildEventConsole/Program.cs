using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildEventConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2) return;

            //CreateZip(args[0], args[1]);
            CreateZipForAssembly(args[0], args[1]);
        }

        static void CreateZip(string sourceDirPath, string targetZipFilePath)
        {
            var targetDirPath = Path.GetDirectoryName(targetZipFilePath);
            Directory.CreateDirectory(targetDirPath);
            File.Delete(targetZipFilePath);
            ZipFile.CreateFromDirectory(sourceDirPath, targetZipFilePath);
        }

        static void CreateZipForAssembly(string sourceAssemblyFilePath, string targetDirPath)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(sourceAssemblyFilePath);
            var assembly = Assembly.LoadFrom(sourceAssemblyFilePath);
            var assemblyFileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (assemblyFileVersion == null) return;

            var sourceDirPath = Path.GetDirectoryName(sourceAssemblyFilePath);
            var targetZipFileName = string.Format("{0}-{1}.zip", assemblyName, assemblyFileVersion.Version);
            var targetZipFilePath = Path.Combine(targetDirPath, targetZipFileName);

            Directory.CreateDirectory(targetDirPath);
            File.Delete(targetZipFilePath);
            ZipFile.CreateFromDirectory(sourceDirPath, targetZipFilePath);
        }
    }
}
