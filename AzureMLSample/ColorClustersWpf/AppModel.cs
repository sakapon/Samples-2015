using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using KLibrary.Labs.ObservableModel;

namespace ColorClustersWpf
{
    public class AppModel
    {
        const string OutputDirPath = @"..\..\..\ColorData\Output";

        public ISettableProperty<string[]> OutputCsvs { get; private set; }
        public ISettableProperty<string> SelectedOutputCsv { get; private set; }
        public IGetOnlyProperty<IDictionary<int, ColorInfo[]>> Assignments { get; private set; }

        public AppModel()
        {
            // ストレージに接続する処理は非同期にしています。
            OutputCsvs = ObservableProperty.CreateSettable(new string[0]);
            SelectedOutputCsv = ObservableProperty.CreateSettable<string>(null);
            Assignments = SelectedOutputCsv.Select(GetAssignments).ToGetOnly(null);

            OutputCsvs.Select(cs => cs.FirstOrDefault()).Subscribe(SelectedOutputCsv);

            Task.Run(() => OutputCsvs.Value = GetOutputCsvs());
        }

        static string[] GetOutputCsvs()
        {
            return Directory.EnumerateFiles(OutputDirPath)
                .Select(Path.GetFileName)
                .ToArray();
        }

        static IDictionary<int, ColorInfo[]> GetAssignments(string csvName)
        {
            var csvPath = Path.Combine(OutputDirPath, csvName);

            var columns = File.ReadLines(csvPath)
                .Take(1)
                .SelectMany(l => l.Split(','))
                .Select((c, i) => new { c, i })
                .ToDictionary(_ => _.c, _ => _.i);

            return File.ReadLines(csvPath)
                .Skip(1)
                .Select(l => l.Split(','))
                .Select(r => new
                {
                    Assignments = Convert.ToInt32(r[columns["Assignments"]]),
                    Info = new ColorInfo { Name = r[columns["Name"]], RGB = r[columns["RGB"]] },
                    Color = ColorTranslator.FromHtml(r[columns["RGB"]]),
                })
                .GroupBy(_ => _.Assignments)
                .OrderBy(g => g.Average(_ => _.Color.GetHue()))
                .ToDictionary(g => g.Key, g => g.Select(_ => _.Info).ToArray());
        }
    }

    public struct ColorInfo
    {
        public string Name { get; set; }
        public string RGB { get; set; }
    }
}
