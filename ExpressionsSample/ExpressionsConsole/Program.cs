using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            EntityTypeTest();
            EntityTypeTimeTest();
            CsvTest();
            TableTest.DoTest();
        }

        static void EntityTypeTest()
        {
            var PersonType = EntityType.Create(new { Id = 0, Name = "", Birthday = DateTime.MinValue });
            // Uses a lambda expression.
            //var PersonType = EntityType.Create(() => new { Id = 0, Name = "", Birthday = DateTime.MinValue });

            var person1 = PersonType.CreateEntity(123, "Taro", new DateTime(2001, 1, 1));
            var person2 = PersonType.CreateEntity(456, "Jiro", new DateTime(2002, 2, 2));

            Console.WriteLine(person1);
            Console.WriteLine(person2);
        }

        static void EntityTypeTimeTest()
        {
            var sw = Stopwatch.StartNew();

            var PersonType = EntityType.Create(new { Id = 0, Name = "", Birthday = DateTime.MinValue });

            Console.WriteLine(sw.Elapsed);

            for (var i = 0; i < 1000000; i++)
                PersonType.CreateEntity(i, "Person", DateTime.MaxValue);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        static void CsvTest()
        {
            var TaskItemType = EntityType.Create(new { Id = 0, Name = "", Rotation = DayOfWeek.Sunday, StartTime = TimeSpan.Zero });

            var query = CsvFile.ReadEntities("tasks.csv", TaskItemType)
                .Where(o => o.Rotation == DayOfWeek.Monday)
                .OrderBy(o => o.StartTime);

            foreach (var item in query)
                Console.WriteLine("{0}: {1}", item.StartTime, item.Name);
        }
    }
}
