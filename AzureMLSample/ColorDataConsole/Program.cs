using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ColorDataConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            var columnNames = "RGB,Name,R,G,B,Hue,Saturation,Brightness";
            var colorData = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(null))
                .Where(c => c.A == 255) // Exclude Transparent.
                .Select(c => string.Join(",", string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B), c.Name, c.R, c.G, c.B, c.GetHue().ToString("N6"), c.GetSaturation().ToString("N6"), c.GetBrightness().ToString("N6")));

            File.WriteAllLines("ColorData.csv", new[] { columnNames }.Concat(colorData));
        }
    }
}
