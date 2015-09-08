using System;
using System.Collections.Generic;
using System.Linq;

namespace ClusteringConsole
{
    public static class Enumerable2
    {
        public static TSource FirstOnMin<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var o = source
                .Select(x => new { x, v = selector(x) })
                .Aggregate((o1, o2) => o1.v <= o2.v ? o1 : o2);
            return o.x;
        }
    }
}
