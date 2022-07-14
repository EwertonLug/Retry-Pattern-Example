using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Retry_Pattern
{
    class Program
    {
        static string urlApi = "http://httpstat.us/200?sleep=1000";
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            
            await Retry(async () => {

                using var response = await httpClient.GetAsync(urlApi);

                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync();
                Console.WriteLine("Response");
            }, 10);
        }



        static async Task Retry(Func<Task> f, int retryTimes = 3, int waitTime = 500)
        {
            for (int i = 0; i < retryTimes; i++)
            {
                try
                {
                    await f();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await Task.Delay(waitTime);
                 
                }
            }
        }
    }
}
