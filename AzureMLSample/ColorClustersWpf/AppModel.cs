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
        public IGetOnlyProperty<ColorCluster[]> Clusters { get; private set; }

        public AppModel()
        {
            // ストレージへの接続は非同期処理です。
            OutputCsvs = ObservableProperty.CreateSettable(new string[0]);
            SelectedOutputCsv = ObservableProperty.CreateSettable<string>(null);
            Clusters = SelectedOutputCsv.Select(GetColorClusters).ToGetOnly(null);

            OutputCsvs.Select(cs => cs.FirstOrDefault()).Subscribe(SelectedOutputCsv);

            Task.Run(() => OutputCsvs.Value = GetOutputCsvs());
        }

        static string[] GetOutputCsvs()
        {
            return Directory.EnumerateFiles(OutputDirPath)
                .Select(Path.GetFileName)
                .ToArray();
        }

        static ColorCluster[] GetColorClusters(string csvName)
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
                    ClusterId = Convert.ToInt32(r[columns["Assignments"]]),
                    Info = new ColorInfo
                    {
                        Name = r[columns["Name"]],
                        RGB = r[columns["RGB"]],
                        Hue = ColorTranslator.FromHtml(r[columns["RGB"]]).GetHue(),
                    },
                })
                .GroupBy(_ => _.ClusterId, _ => _.Info)
                .OrderBy(g => g.Average(c => c.Hue))
                .Select(g => new ColorCluster
                {
                    Id = g.Key,
                    Colors = g.OrderBy(c => c.Hue).ToArray(),
                })
                .ToArray();
        }
    }

    public struct ColorCluster
    {
        public int Id { get; set; }
        public ColorInfo[] Colors { get; set; }
    }

    public struct ColorInfo
    {
        public string Name { get; set; }
        public string RGB { get; set; }
        public float Hue { get; set; }
    }
}
