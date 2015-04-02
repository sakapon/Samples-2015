using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using KLibrary.Labs.ObservableModel;

namespace ColorClustersWpf
{
    public class AppModel
    {
        const string OutputDirPath = @"..\..\..\ColorData\Output";

        public string[] OutputCsvs { get; private set; }
        public ISettableProperty<string> SelectedOutputCsv { get; private set; }

        public IGetOnlyProperty<IDictionary<int, ColorInfo[]>> Assignments { get; private set; }

        public AppModel()
        {
            OutputCsvs = Directory.EnumerateFiles(OutputDirPath)
                .Select(Path.GetFileName)
                .ToArray();
            SelectedOutputCsv = ObservableProperty.CreateSettable(OutputCsvs.FirstOrDefault());

            Assignments = SelectedOutputCsv.SelectToGetOnly(ReadAssignments);
        }

        static IDictionary<int, ColorInfo[]> ReadAssignments(string csvName)
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
