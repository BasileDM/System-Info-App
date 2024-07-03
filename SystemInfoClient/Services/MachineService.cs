using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using SystemInfoClient.Classes.System;
using System.Runtime.Versioning;

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
            // Build HTTP Client and add authorization header with token
            HttpClient client = HttpClientFactory.CreateHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize machine into JSON content, build route string, and send
            // If the machine ID is 0 it is a new machine
            var content = new StringContent(machine.JsonSerialize(), Encoding.UTF8, "application/json");
            string route = machine.Id == 0 ? $"{_apiUrl}api/Machines/Create" : $"{_apiUrl}api/Machines/Update/{machine.Id}";

            return machine.Id == 0 ? await client.PostAsync(route, content) : await client.PutAsync(route, content);
        }
        public async Task<bool> IsResponseOkAsync(HttpResponseMessage response, MachineClass machine)
        {
            if (response.IsSuccessStatusCode && response.Headers.Location != null)
            {
                // Display response data 
                Console.WriteLine();
                Console.WriteLine($"Machine data sent successfully.");
                Console.WriteLine($"Code: {response.StatusCode}.");
                Console.WriteLine($"Time: {response.Headers.Date}.");
                Console.WriteLine($"Location: {response.Headers.Location}.");

                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Try to display relevant authorization error message.
                Console.WriteLine("Authorization error.");
                if (response.Headers.WwwAuthenticate.Count > 0)
                {
                    var wwwAuthHeader = response.Headers.WwwAuthenticate.ToString();
                    var errorDescriptionIndex = wwwAuthHeader.IndexOf("error_description=");

                    if (errorDescriptionIndex != -1)
                    {
                        var errorDescription = wwwAuthHeader.Substring(errorDescriptionIndex + "error_description=".Length);
                        Console.WriteLine($"Error Description: {errorDescription}");
                    }
                }
                else
                {
                    Console.WriteLine($"Unexpected authorization error.");
                }

                // Try to obtain a new token
                var newToken = await _securityService.GetOrRequestTokenAsync();
                HttpResponseMessage retryResponse = await SendMachineInfoAsync(machine, newToken);

                return retryResponse.IsSuccessStatusCode;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine();
                Console.WriteLine($"{response.ReasonPhrase}: {errorContent}");
                return false;
            }
        }
    }

    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
