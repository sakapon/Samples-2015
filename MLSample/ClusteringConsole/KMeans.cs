using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ClusteringConsole
{
    public class KMeans<T>
    {
        public int Clusters { get; private set; }
        public int Iterations { get; private set; }

        public KMeans(int clusters, int iterations)
        {
            Clusters = clusters;
            Iterations = iterations;
        }

        public Dictionary<int, Record<T>[]> Execute(Record<T>[] records)
        {
            throw new NotImplementedException();
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    public struct Record<T>
    {
        public T Element { get; set; }
        public double[] Features { get; set; }

        string ToDebugString()
        {
            return string.Format("{0}: {1}", Element, FeaturesHelper.ToString(Features));
        }
    }

    [DebuggerDisplay(@"\{{ToDebugString()}\}")]
    class Cluster<T>
    {
        public int Id { get; private set; }
        public double[] Centroid { get; private set; }
        public List<Record<T>> Records { get; private set; }

        public Cluster(int id, double[] centroid)
        {
            Id = id;
            Centroid = centroid;
            Records = new List<Record<T>>();
        }

        public void TuneCentroid()
        {
            Centroid = Enumerable.Range(0, Centroid.Length)
                .Select(i => Records.Average(r => r.Features[i]))
                .ToArray();
        }

        string ToDebugString()
        {
            return string.Format("{0}: {1}: {2} records", Id, FeaturesHelper.ToString(Centroid), Records.Count);
        }
    }

    public static class FeaturesHelper
    {
        public static double GetDistance(double[] p1, double[] p2)
        {
            return Math.Sqrt(p1.Zip(p2, (x1, x2) => x1 - x2).Sum(x => x * x));
        }

        public static double GetNorm(double[] p)
        {
            return Math.Sqrt(p.Sum(x => x * x));
        }

        public static string ToString(double[] p)
        {
            return string.Join(", ", p.Select(x => x.ToString("F3")));
        }
    }
}
