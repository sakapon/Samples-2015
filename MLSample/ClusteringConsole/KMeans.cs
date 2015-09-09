using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ClusteringConsole
{
    [DebuggerDisplay(@"\{Clusters: {ClustersNumber}, Iterations: {IterationsNumber}\}")]
    public class KMeans<T>
    {
        public int ClustersNumber { get; private set; }
        public int IterationsNumber { get; private set; }

        public KMeans(int clustersNumber, int iterationsNumber)
        {
            ClustersNumber = clustersNumber;
            IterationsNumber = iterationsNumber;
        }

        public Dictionary<int, Record<T>[]> Train(Record<T>[] records)
        {
            var clusters = InitializeClusters(records);

            for (var i = 0; i < IterationsNumber; i++)
                TrainOnce(clusters, records);

            return clusters.ToDictionary(c => c.Id, c => c.Records.ToArray());
        }

        Cluster<T>[] InitializeClusters(Record<T>[] records)
        {
            return RandomUtility.ShuffleRange(records.Length)
                .Take(ClustersNumber)
                .Select(i => records[i])
                .Select((r, i) => new Cluster<T>(i, r.Features))
                .ToArray();
        }

        void TrainOnce(Cluster<T>[] clusters, Record<T>[] records)
        {
            Array.ForEach(clusters, c => c.Records.Clear());
            AssignRecords(clusters, records);
            Array.ForEach(clusters, c => c.TuneCentroid());
        }

        static void AssignRecords(Cluster<T>[] clusters, IEnumerable<Record<T>> records)
        {
            foreach (var record in records)
            {
                var cluster = clusters.FirstOnMin(c => FeaturesHelper.GetDistance(c.Centroid, record.Features));
                cluster.Records.Add(record);
            }
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
