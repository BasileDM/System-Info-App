using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using SystemInfoClient.Classes;
using SystemInfoClient.Models;
using SystemInfoClient.Services;

namespace SystemInfoClient
{
    [SupportedOSPlatform("windows")]
    public class SystemInfoClient
    {
        public static async Task Main()
        {
            //Read config file to get customer_id
            Settings settings = LoadConfig();
            int customerId = Convert.ToInt32(settings.CustomerId);

            if (customerId == 0 || customerId < 0)
            {
                throw new Exception("Customer ID is invalid, please provide a valid one in the settings.json file");
            }

            // Instantiate object with machine info and customer ID from settings file
            MachineClass machine = new() { CustomerId = customerId };

            // Log information
            machine.LogInfo();
            ApplicationsService.LogExeInfo(settings.Applications["AnyDesk"]);

            //Serialize and send object to POST API route
            await PostMachineInfo(machine);

        }

        private static async Task PostMachineInfo(MachineClass machine)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

            string route = "https://localhost:7056/api/Machines/Create";

            JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
            var json = JsonSerializer.Serialize(machine, jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(route, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"\r\n" +
                    $"Machine data sent successfully.\r\n" +
                    $"Code: {response.StatusCode}.\r\n" +
                    $"Time: {response.Headers.Date}\r\n" +
                    $"Location: {response.Headers.Location}"
                );
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"\r\n" +
                    $"{response.ReasonPhrase}: {errorContent}");
            }
        }

        private static Settings LoadConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "settings.json";
            string jsonSettings;

            try
            {
                using (StreamReader reader = new(path))
                {
                    jsonSettings = reader.ReadToEnd();
                }

                Settings? settings = JsonSerializer.Deserialize<Settings>(jsonSettings);

                return settings;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"File not found: {ex}");
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Could not deserialize JSON: {ex}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while trying to read settings file: {ex}");
            }
        }
    }
}