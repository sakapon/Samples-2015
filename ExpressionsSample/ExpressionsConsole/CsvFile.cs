using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExpressionsConsole
{
    public static class CsvFile
    {
        public static IEnumerable<Dictionary<string, string>> ReadLines(string path, Encoding encoding = null)
        {
            // Simple implementation.
            var lines = File.ReadLines(path, encoding ?? Encoding.UTF8).Select(l => l.Split(','));
            string[] columnNames = null;

            foreach (var line in lines)
            {
                if (columnNames == null)
                    columnNames = line;
                else
                    yield return columnNames.Zip(line, (c, v) => new { c, v }).ToDictionary(o => o.c, o => o.v);
            }
        }

        public static IEnumerable<TEntity> ReadEntities<TEntity>(string path, EntityType<TEntity> entityType, Encoding encoding = null)
        {
            if (entityType == null) throw new ArgumentNullException("entityType");

            var parameters = entityType.ConstructorInfo.GetParameters();

            return ReadLines(path, encoding)
                .Select(d => parameters.Select(p => d[p.Name].To(p.ParameterType)).ToArray())
                .Select(p => entityType.CreateEntity(p));
        }
    }
}
