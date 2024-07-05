using System.Net.Http.Headers;

namespace SystemInfoClient.Services
{
    internal class HttpClientFactory
    {
        public static HttpClient CreateHttpClient()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

            return client;
        }
    }
}
