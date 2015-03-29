using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ColorDataConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            CreateColorDataJP();
        }

        static void CreateColorData()
        {
            var columnNames = "RGB,Name,R,G,B,Hue,Saturation,Brightness";
            var colorData = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(null))
                .Where(c => c.A == 255) // Exclude Transparent.
                .Select(c => string.Join(",", string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B), c.Name, c.R, c.G, c.B, c.GetHue().ToString("N6"), c.GetSaturation().ToString("N6"), c.GetBrightness().ToString("N6")));

            File.WriteAllLines("ColorData.csv", new[] { columnNames }.Concat(colorData));
        }

        static void CreateColorDataJP()
        {
            var columnNames = "RGB,Name,RomanName,R,G,B,Hue,Saturation,Brightness";
            var colorData = File.ReadLines(@"..\..\..\ColorData\ColorData-JP-org.csv")
                .Skip(1)
                .Select(l => l.Split(','))
                .Select(org => new { org, c = ColorTranslator.FromHtml(org[0]) })
                .Select(_ => string.Join(",", _.org[0], _.org[1], _.org[2], _.c.R, _.c.G, _.c.B, _.c.GetHue().ToString("N6"), _.c.GetSaturation().ToString("N6"), _.c.GetBrightness().ToString("N6")));

            File.WriteAllLines("ColorData-JP.csv", new[] { columnNames }.Concat(colorData), Encoding.UTF8);
        }
    }
}
