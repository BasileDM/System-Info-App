using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using SystemInfoClient.Classes.System;
using System.Runtime.Versioning;
using SystemInfoClient.Classes;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class NetworkService
    {
        private readonly string _apiUrl;

        public NetworkService(SettingsClass settings)
        {
            _apiUrl = settings.ApiUrl ?? throw new Exception("Invalid API URL in settings.json");
        }

        private static HttpClient CreateHttpClient()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

            return client;
        }
        private async Task<string> GetJwtToken()
        {
            try
            {
                // Prepare and send request
                HttpClient client = CreateHttpClient();

                var hashedPass = SecurityService.GetPasswordHash(out byte[] salt);

                Console.WriteLine($"Token requested with password: \r\nPass: {hashedPass}\r\nSalt: {Convert.ToHexString(salt)}");

                var authRequest = new { Pass = hashedPass, Salt = salt };
                var content = new StringContent(JsonSerializer.Serialize(authRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(_apiUrl + "api/Auth/GetToken", content);

                // Handle response
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

                    if (tokenResponse != null && tokenResponse.Token.ToString() != null)
                    {
                        Console.WriteLine($"Token obtained with success:\r\n{tokenResponse.Token}");
                        return tokenResponse.Token;
                    }
                    else
                    {
                        throw new Exception("Null token.");
                    }
                }
                else
                {
                    throw new Exception($"Failed to obtain authentication token: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                throw new Exception("Failed to obtain authentication token due to an HTTP request error.", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON serialization/deserialization error: {ex.Message}");
                throw new Exception("Failed to parse the token response from the API.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred while obtaining the authentication token.", ex);
            }
        }
        public async Task<HttpResponseMessage> PostMachineInfo(MachineClass machine)
        {
            // Fetch JWT token
            string token = await GetJwtToken();

            // Build HTTP Client and add authorization header with token
            HttpClient client = CreateHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize machine into JSON content and build route string
            var content = new StringContent(machine.JsonSerialize(), Encoding.UTF8, "application/json");

            // Send to API
            // If the machine ID is 0, then it is a new machine
            if (machine.Id == 0)
            {
                string route = _apiUrl + "api/Machines/Create";
                return await client.PostAsync(route, content);
            }
            // If the machine already has an ID, use the update route
            else if (machine.Id > 0)
            {
                string route = _apiUrl + "api/Machines/Update/" + machine.Id;
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
    }

    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
