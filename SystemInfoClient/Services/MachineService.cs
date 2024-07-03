using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using SystemInfoClient.Classes.System;
using System.Runtime.Versioning;
using SystemInfoClient.Classes;
using System.Net;
using SystemInfoApi.Utilities;

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
            Logger.WriteColored("Sending machine info...", ConsoleColor.Yellow);

            // Build HTTP Client and add authorization header with token
            HttpClient client = HttpClientFactory.CreateHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize machine into JSON content, build route string, and send
            // If the machine ID is 0 it is a new machine
            var content = new StringContent(machine.JsonSerialize(), Encoding.UTF8, "application/json");

            string route = machine.Id == 0 ?
                $"{_apiUrl}api/Machines/Create" :
                $"{_apiUrl}api/Machines/Update/{machine.Id}";

            return machine.Id == 0 ?
                await client.PostAsync(route, content) :
                await client.PutAsync(route, content);
        }
        public async Task HandleResponseAsync(HttpResponseMessage response, MachineClass machine, SettingsClass settings)
        {
            switch (response.StatusCode)
            {
                // Machine creation
                case HttpStatusCode.OK when response.Headers.Location != null:
                    Logger.LogSuccessDetails(response);
                    UpdateSettingsWithId(response, settings);
                    break;

                // Machine update
                case HttpStatusCode.Created:
                    Logger.LogSuccessDetails(response);
                    break;

                // Unauthorized
                case HttpStatusCode.Unauthorized:
                    Logger.LogAuthorizationError(response);

                    // Try to obtain a new token
                    var newToken = await _securityService.RequestTokenAsync();

                    // Send machine info with new token
                    HttpResponseMessage retryResponse = await SendMachineInfoAsync(machine, newToken);

                    retryResponse.EnsureSuccessStatusCode();
                    Logger.LogSuccessDetails(retryResponse);
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
        private static void UpdateSettingsWithId(HttpResponseMessage response, SettingsClass settings)
        {
            string newMachineId = GetMachineIdFromResponse(response);

            if (newMachineId != settings.ParsedMachineId.ToString())
                settings.RewriteFileWithId(newMachineId);
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

    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
