using System.Net.Http.Headers;
using System.Text;
using System.Runtime.Versioning;
using System.Net;
using SystemInfoClient.Utilities;
using SystemInfoClient.Classes.System;
using SystemInfoClient.Classes;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class MachineService
    {
        private readonly string _apiUrl;
        private readonly SecurityService _securityService;

        public MachineService(string apiUrl, SecurityService securityService)
        {
            _apiUrl = apiUrl ?? throw new Exception("Invalid API URL in settings.json");
            _securityService = securityService;
        }

        public async Task<HttpResponseMessage> SendMachineInfoAsync(MachineClass machine, string token)
        {
            ConsoleUtils.WriteColored("Sending machine info...", ConsoleColor.Yellow);

            // Build HTTP Client and add authorization header with token
            HttpClient client = HttpClientFactory.CreateHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize machine into JSON content, build route string, and send
            // If the machine ID is 0 it is a new machine
            var content = new StringContent(machine.JsonSerialize(), Encoding.UTF8, "application/json");

            string route = machine.Id == 0 ?
                $"{_apiUrl}api/Machines/Create" :
                $"{_apiUrl}api/Machines/Update/{machine.Id}";

            Console.WriteLine($"Route used: {route}");
            Console.WriteLine($"ID machine: {machine.Id}");

            return machine.Id == 0 ?
                await client.PostAsync(route, content) :
                await client.PutAsync(route, content);
        }
        public async Task HandleResponseAsync(HttpResponseMessage response, MachineClass machine, Settings settings)
        {
            switch (response.StatusCode)
            {
                // Machine creation
                case HttpStatusCode.Created when response.Headers.Location != null:
                    Console.WriteLine("Creation detected.");
                    ConsoleUtils.LogSuccessDetails(response);
                    UpdateSettingsWithId(response, settings);
                    break;

                // Machine update
                case HttpStatusCode.OK:
                    Console.WriteLine("Update detected.");
                    ConsoleUtils.LogSuccessDetails(response);
                    break;

                // Unauthorized
                case HttpStatusCode.Unauthorized:
                    Console.WriteLine("Unauthorized detected.");
                    ConsoleUtils.LogAuthorizationError(response);

                    // Try to obtain a new token
                    JwtToken newToken = await _securityService.RequestTokenAsync();

                    // Send machine info with new token
                    machine.LogJson();
                    HttpResponseMessage retryResponse = await SendMachineInfoAsync(machine, newToken.GetString());

                    retryResponse.EnsureSuccessStatusCode();
                    ConsoleUtils.LogSuccessDetails(retryResponse);
                    UpdateSettingsWithId(retryResponse, settings);
                    break;

                // Generic
                default:
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine();
                    Console.WriteLine($"{response.ReasonPhrase}: {errorContent}");
                    break;
            }
        }
        private static void UpdateSettingsWithId(HttpResponseMessage response, Settings settings)
        {
            string? newMachineId = GetMachineIdFromResponse(response);

            if (newMachineId != settings.ParsedMachineId.ToString() && newMachineId != null)
            {
                settings.RewriteFileWithId(newMachineId);
            }
            else
            {
                Console.WriteLine("Machine ID not updated in settings.json: This is normal for a machine update.");
            }
        }
        private static string? GetMachineIdFromResponse(HttpResponseMessage response)
        {
            // Parse the last element of the Location header in the response to get the new machine ID
            string machineId;
            if (response.Headers.Location != null)
            {
                machineId = response.Headers.Location.Segments.Last();
            }
            else
            {
                return null;
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
