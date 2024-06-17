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
            try
            {
                // Load config file to get customer ID
                SettingsModel settings = LoadConfig();

                int customerId = Int32.TryParse(settings.CustomerId, out int parsedId) ? parsedId : 0;

                if (customerId <= 0)
                {
                    throw new Exception("Invalid customer ID, please provide a valid one in the settings.json file");
                }

                if (settings != null && settings.ApplicationsList != null)
                {
                    // Instantiate object with machine info and customer ID from settings file
                    MachineClass machine = new(settings.ApplicationsList) { CustomerId = customerId };

                    // Log information
                    machine.LogInfo();
                    //ApplicationsService.LogAppListInfo(settings.ApplicationsList);

                    // Serialize and send object to POST API route
                    //await PostMachineInfo(machine, settings.ApiUrl);
                }
                else
                {
                    throw new NullReferenceException("Error: Null configuration");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task PostMachineInfo(MachineClass machine, string? ApiUrl)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

            JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
            var json = JsonSerializer.Serialize(machine, jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string route = ApiUrl + "api/Machines/Create";

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

        private static SettingsModel LoadConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "settings.json";
            string jsonSettings;

            try
            {
                using (StreamReader reader = new(path))
                {
                    jsonSettings = reader.ReadToEnd();
                }

                SettingsModel? settings = JsonSerializer.Deserialize<SettingsModel>(jsonSettings);

                if (settings == null || settings.ApplicationsList == null)
                {
                    throw new NullReferenceException("Settings deserialization error, config or applist is null.");
                }

                int customerId = Int32.TryParse(settings.CustomerId, out int parsedId) ? parsedId : 0;
                if (customerId <= 0)
                {
                    throw new InvalidDataException("Invalid customer ID, please provide a valid one in the settings.json file");
                }

                return settings;
            }
            catch (NullReferenceException ex)
            {
                throw new NullReferenceException(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"File not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Could not deserialize JSON: {ex.Message}");
            }
            catch (InvalidDataException ex)
            {
                throw new InvalidDataException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while trying to read settings file: {ex.Message}");
            }
        }
    }
}