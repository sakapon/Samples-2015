using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ColorClustersApiConsole
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class Program
    {
        const string apiUri = "abc123"; // Replace this with the API URI for the web service
        const string apiKey = "abc123"; // Replace this with the API key for the web service

        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static async Task InvokeRequestResponseService()
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, StringTable>
                    {
                        {
                            "input1",
                            new StringTable
                            {
                                ColumnNames = new string[] {"RGB", "Name", "R", "G", "B", "Hue", "Saturation", "Brightness"},
                                Values = new string[,]
                                {
                                    { "#005CAF", "瑠璃", "0", "0", "0", "208.457100", "0", "0" },
                                    { "#CA7A2C", "琥珀", "0", "0", "0", "29.620250", "0", "0" },
                                    { "#1C1C1C", "墨", "0", "0", "0", "0", "0", "0" },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>(),
                };

                client.BaseAddress = new Uri(apiUri);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
