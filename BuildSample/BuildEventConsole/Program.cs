using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuildEventConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2) return;

            CreateZip(args[0], args[1]);
            //CreateZipForAssembly(args[0], args[1]);
            //IncrementVersion(args[0]);
        }

        public static void CreateZip(string sourceDirPath, string targetZipFilePath)
        {
            var targetDirPath = Path.GetDirectoryName(targetZipFilePath);
            Directory.CreateDirectory(targetDirPath);
            File.Delete(targetZipFilePath);
            ZipFile.CreateFromDirectory(sourceDirPath, targetZipFilePath);
        }

        public static void CreateZipForAssembly(string sourceAssemblyFilePath, string targetDirPath)
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

        public static void IncrementVersion(string projDirPath)
        {
            var assemblyInfoPath = Directory.EnumerateFiles(projDirPath, "AssemblyInfo.cs", SearchOption.AllDirectories).First();
            var contents = File.ReadLines(assemblyInfoPath, Encoding.UTF8)
                .Select(IncrementLine)
                .ToArray();
            File.WriteAllLines(assemblyInfoPath, contents, Encoding.UTF8);
        }

        static string IncrementLine(string line)
        {
            if (line.StartsWith("//")) return line;

            var match = Regex.Match(line, @"Assembly(File)?Version\(""([0-9\.]+)""\)");
            if (!match.Success) return line;

            var oldVersion = match.Groups[2].Value;
            var newVersion = IncrementBuildNumber(oldVersion);
            return line.Replace(oldVersion, newVersion);
        }

        static string IncrementBuildNumber(string version)
        {
            return Regex.Replace(version, @"^(\d+\.\d+\.)(\d+)((\.\d+)?)$", m => m.Groups[1].Value + IncrementNumber(m.Groups[2].Value) + m.Groups[3].Value);
        }

        static string IncrementNumber(string i)
        {
            return (int.Parse(i) + 1).ToString();
        }
    }
}
