using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using SystemInfoClient.Classes;
using SystemInfoClient.Models;

namespace SystemInfoClient
{
    [SupportedOSPlatform("windows")]
    public class SystemInfoClient
    {
        public static async Task Main()
        {
            try
            {
                // Load settings.json and check 'CustomerId' validity
                SettingsModel settings = LoadSettings();
                int customerId = GetParsedId(settings.CustomerId);

                // Create machine with 'CustomerId' + drives, os and apps info
                MachineClass machine = new(settings) { CustomerId = customerId };
                machine.LogJson();

                // POST machine to API route and handle response
                HandleApiResponse(await PostMachineInfo(machine, settings.ApiUrl));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string GetSettingsPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "settings.json";
        }

        private static SettingsModel LoadSettings()
        {
            try
            {
                string jsonSettings;

                using (StreamReader reader = new(GetSettingsPath()))
                {
                    jsonSettings = reader.ReadToEnd();
                }

                SettingsModel? settings = JsonSerializer.Deserialize<SettingsModel>(jsonSettings);

                if (settings == null || settings.ApplicationsList == null)
                    throw new NullReferenceException("Settings deserialization error, config or applist is null.");

                return settings;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"File not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Could not deserialize JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while trying to read settings file: {ex.Message}");
            }
        }

        private static int GetParsedId(string? id)
        {
            if (Int32.TryParse(id, out int parsedId) && parsedId > 0) return parsedId;

            else throw new InvalidDataException(
                "Invalid customer ID, please provide a valid one in the settings.json file");
        }

        private static async Task<HttpResponseMessage> PostMachineInfo(MachineClass machine, string? ApiUrl)
        {
            // Build HTTP Client
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

            // Serialize machine into JSON content and build route string
            var content = new StringContent(machine.JsonSerialize(), Encoding.UTF8, "application/json");
            string route = ApiUrl + "api/Machines/Create";

            // POST to API route and handle the response
            return await client.PostAsync(route, content);
        }

        public async static void HandleApiResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode && response.Headers.Location != null)
            {
                // Display console info
                Console.WriteLine(
                    $"\r\n" +
                    $"Machine data sent successfully.\r\n" +
                    $"Code: {response.StatusCode}.\r\n" +
                    $"Time: {response.Headers.Date}\r\n" +
                    $"Location: {response.Headers.Location}\r\n"
                );

                // Get machine ID by parsing the last element of the Location header
                string machineId = response.Headers.Location.Segments.Last();
                if (Int32.TryParse(machineId, out int parsedMachineId) 
                    && parsedMachineId >0)
                {
                    RewriteMachineIdSettings(machineId); // Insert it in the settings.json
                }
                else
                {
                    throw new InvalidDataException("The machine ID sent by the API was invalid.");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"\r\n" +
                    $"{response.ReasonPhrase}: {errorContent}");
            }
        }

        public static void RewriteMachineIdSettings(string machineId)
        {
            try
            {
                string json = File.ReadAllText(GetSettingsPath());
                SettingsModel settings = JsonSerializer.Deserialize<SettingsModel>(json);
                settings.MachineId = machineId;

                string newJson = JsonSerializer.Serialize(settings); // add the write indented and refactor to avoid having to create settings again when we have in in main, pass the argument from the main method maybe ?
                string path = GetSettingsPath();

                File.WriteAllText(path, newJson);
                Console.WriteLine($"New machien id: {machineId}, path {path} newjson : \r\n{newJson}");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"{ex.Message} + {ex}");
            }
        }
    }
}