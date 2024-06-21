using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text;
using SystemInfoClient.Classes;

namespace SystemInfoClient
{
    [SupportedOSPlatform("windows")]
    public class SystemInfoClient
    {
        public static async Task Main()
        {
            try
            {
                // Load settings.json from SettingsClass factory method
                SettingsClass settings = SettingsClass.GetInstance();

                // Create full machine with CustomerId, drives, os and apps info
                MachineClass machine = new(settings);
                machine.LogJson();

                // POST machine to API route
                HttpResponseMessage response = await PostMachineInfo(machine, settings.ApiUrl);

                // Handle API response
                if (await IsResponseOk(response))
                {
                    string newMachineId = GetMachineIdFromResponse(response);

                    if (newMachineId != settings.ParsedMachineId.ToString()) 
                        settings.RewriteFileWithId(newMachineId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

            // Send to API
            // If the machine ID is 0, then it is a new machine
            if (machine.Id == 0)
            {

                string route = ApiUrl + "api/Machines/Create";
                return await client.PostAsync(route, content);
            }
            // If the machine already has an ID, use the update route
            else if (machine.Id > 0)
            {
                string route = ApiUrl + "api/Machines/Update/" + machine.Id;
                return await client.PutAsync(route, content);
            }
            else
            {
                throw new InvalidDataException("Could not post the machine to the API, the ID was not valid.");
            }
        }
        public async static Task<bool> IsResponseOk(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode && response.Headers.Location != null)
            {
                // Display response data 
                Console.WriteLine(
                    $"\r\n" +
                    $"Machine data sent successfully.\r\n" +
                    $"Code: {response.StatusCode}.\r\n" +
                    $"Time: {response.Headers.Date}\r\n" +
                    $"Location: {response.Headers.Location}\r\n"
                );

                return true;
            }
            else
            {
                // Display response's error content
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"\r\n" +
                    $"{response.ReasonPhrase}: {errorContent}");

                return false;
            }
        }
        private static string GetMachineIdFromResponse(HttpResponseMessage response)
        {
            // Parse the last element of the Location header in the response to get the new machine ID
            string machineId;
            if (response.Headers.Location != null)
            {
                machineId = response.Headers.Location.Segments.Last(); 
            }
            else
            {
                throw new InvalidDataException("Invalid API response's location header");
            }

            if (Int32.TryParse(machineId, out int parsedMachineId) && parsedMachineId > 0)
            {
                return machineId;
            }
            else
            {
                throw new InvalidDataException("The machine ID sent by the API was invalid.");
            }
        }
    }
}