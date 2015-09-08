using System;
using System.Collections.Generic;
using System.Linq;

namespace ClusteringConsole
{
    public static class RandomUtility
    {
        static Random _random = new Random();

        public static double NextDouble(double minValue, double maxValue)
        {
            return minValue + (maxValue - minValue) * _random.NextDouble();
        }

        public static IEnumerable<int> ShuffleRange(int maxValue)
        {
            if (maxValue < 0) throw new ArgumentOutOfRangeException("maxValue", "maxValue is less than 0.");

            return ShuffleRange(0, maxValue);
        }

        public static IEnumerable<int> ShuffleRange(int minValue, int maxValue)
        {
            if (maxValue < minValue) throw new ArgumentOutOfRangeException("maxValue", "maxValue is less than minValue.");

            var l = Enumerable.Range(minValue, maxValue - minValue).ToList();

            while (l.Count > 0)
            {
                var index = _random.Next(l.Count);
                yield return l[index];
                l.RemoveAt(index);
            }
        }
    }
}
