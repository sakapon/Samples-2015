using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClusteringConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ClusterColors();
            //ClusterPoints();
        }

        static void ClusterColors()
        {
            var colors = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => (Color)p.GetValue(null))
                .Where(c => c.A == 255) // Exclude Transparent.
                .ToArray();

            var records = colors
                .Select(c => new Record<Color> { Element = c, Features = new double[] { c.R, c.G, c.B } })
                .ToArray();

            var clustering = new KMeans<Color>(20, 50);
            var clusters = clustering.Execute(records);

            foreach (var cluster in clusters)
            {
                Console.WriteLine(cluster.Key);
                Console.WriteLine(string.Join(", ", cluster.Value.Select(r => r.Element.Name)));
            }
        }

        static void ClusterPoints()
        {
            var points = Enumerable.Range(0, 100)
                .Select(_ => Enumerable.Range(1, 3).Select(i => RandomUtility.NextDouble(0, i)).ToArray())
                .ToArray();

            var records = points
                .Select((p, i) => new Record<int> { Element = i, Features = p, })
                .ToArray();

            var clustering = new KMeans<int>(10, 20);
            var clusters = clustering.Execute(records);

            foreach (var cluster in clusters)
            {
                Console.WriteLine(cluster.Key);
                Array.ForEach(cluster.Value, r => Console.WriteLine(FeaturesHelper.ToString(r.Features)));
            }
        }
    }
}
