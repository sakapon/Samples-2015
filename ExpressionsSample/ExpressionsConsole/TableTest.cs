using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ExpressionsConsole
{
    public static class TableTest
    {
        static readonly CloudTable PeopleTable;

        static TableTest()
        {
            var accountString = ConfigurationManager.ConnectionStrings["StorageAccount"].ConnectionString;
            var account = CloudStorageAccount.Parse(accountString);
            var tableClient = account.CreateCloudTableClient();
            PeopleTable = tableClient.GetTableReference("people");
        }

        public static void DoTest()
        {
            Where_NoHelper();
            RetrieveTest();
            SelectTest();
            WhereTest1();
            WhereTest2();
            WhereSelectTest();
        }

        public static void Where_NoHelper()
        {
            var query = new TableQuery<Person>()
                .Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "2015"),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForInt("Age", QueryComparisons.LessThan, 20)));

            var result = PeopleTable.ExecuteQuery(query).ToArray();
        }

        public static void RetrieveTest()
        {
            var result = PeopleTable.Retrieve<Person>("2015", "123");
        }

        public static void SelectTest()
        {
            var query = new TableQuery<Person>()
                .Select(p => new { p.FirstName, p.Age });

            var result = PeopleTable.ExecuteQuery(query).ToArray();
        }

        public static void WhereTest1()
        {
            var query = new TableQuery<Person>()
                .Where(p => p.PartitionKey == "2015" && (p.LastName.CompareTo("W") >= 0 || p.Age < 20));

            var result = PeopleTable.ExecuteQuery(query).ToArray();
        }

        public static void WhereTest2()
        {
            var query = new TableQuery<Person>()
                .Where(p => p.PartitionKey == "2015")
                .Where(p => p.LastName.CompareTo("W") >= 0 || p.Age < 20);

            var result = PeopleTable.ExecuteQuery(query).ToArray();
        }

        public static void WhereSelectTest()
        {
            var query = new TableQuery<Person>()
                .Where(p => p.PartitionKey == "2015")
                .Where(p => p.LastName.CompareTo("W") >= 0 || p.Age < 20)
                .Select(p => new { p.FirstName, p.Age });

            var result = PeopleTable.ExecuteQuery(query).ToArray();
        }
    }

    [DebuggerDisplay(@"\{{PartitionKey}/{RowKey}\}")]
    public class Person : TableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
