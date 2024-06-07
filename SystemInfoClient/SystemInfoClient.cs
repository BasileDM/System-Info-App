using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using SystemInfoClient.Classes;
using SystemInfoClient.Services;

namespace SystemInfoClient
{
    [SupportedOSPlatform("windows")]
    public class SystemInfoClient
    {
        public static async Task Main(string[] args) {

            //Read config file to get customer_id
            int customerId = 1;

            // Get machine information into instantiated object
            MachineClass machine = new();

            //Serialize and send object to POST API route
            await PostMachineInfo(machine, customerId);

            //string appPath = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe";
            //ApplicationsService.LogExeInfo(appPath);
        }

        public static async Task PostMachineInfo(MachineClass machine, int customerId) {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

            var route = "https://localhost:7056/api/Machines/Create";

            JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
            var json = JsonSerializer.Serialize(machine, jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(route, content);

            if (response.IsSuccessStatusCode) {
                Console.WriteLine(
                    $"Machine data sent successfully.\r\n" +
                    $"Code: {response.StatusCode}.\r\n" +
                    $"Time: {response.Headers.Date}\r\n" +
                    $"Location: {response.Headers.Location}"
                );
            } else {
                Console.WriteLine($"Post request failed: {response.StatusCode}");
            }
        }
    }
}