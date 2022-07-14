using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Retry_Pattern
{
    class Program
    {
        static string _urlApi = default;
        static HttpClient _httpClient;
        static async Task Main(string[] args)
        {

            _httpClient = new HttpClient();
            //Example 1
            try
            {
                Console.WriteLine("Retry without return ================>");
                _urlApi = "http://htttpstat.us/200?sleep=1000";
                await Retry(RequestAsync, 10);
            }
            catch (Exception)
            {
                Console.WriteLine("All attempts failed");
            }
            //Example 2
            try
            {
              
                Console.WriteLine("Retry with returns ================>");
                _urlApi = "http://worldtimeapi.org/api/timezone/America/Sao_Paulo";
                string result = await Retry(RequestWithReturnAsync, 3);
                Console.WriteLine(result);
            }
            catch (Exception)
            {
                Console.WriteLine("All attempts failed");
            }

        }

        private static async Task RequestAsync()
        {
            using var response = await _httpClient.GetAsync(_urlApi);
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync();
            Console.WriteLine("Response received");

        }

        private static async Task<string> RequestWithReturnAsync()
        {
            using var response = await _httpClient.GetAsync(_urlApi);
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        static async Task Retry(Func<Task> f, int retryTimes = 3, int waitTime = 500)
        {
            for (int i = 0; i < retryTimes - 1; i++)
            {
                try
                {
                    await f();
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await Task.Delay(waitTime);

                }
            }

            await f();
        }

        static async Task<T> Retry<T>(Func<Task<T>> f, int retryTimes = 3, int waitTime = 500)
        {
            for (int i = 0; i < retryTimes - 1; i++)
            {
                try
                {
                    return await f();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await Task.Delay(waitTime);

                }
            }

            return await f();
        }
    }
}
