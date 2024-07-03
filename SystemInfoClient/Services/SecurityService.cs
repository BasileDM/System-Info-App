using Konscious.Security.Cryptography;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class SecurityService
    {
        private readonly string _apiUrl;
        private readonly string _envName = "SysInfoApp";

        public SecurityService(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<string> GetOrRequestTokenAsync()
        {
            string? token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                var hash = GetPasswordHash(out byte[] salt);
                token = await RequestTokenAsync(hash, salt);
                StoreToken(token);
            }

            return token;
        }
        public async Task<string> RequestTokenAsync(string hash, byte[] salt)
        {
            try
            {
                // Prepare and send request
                HttpClient client = HttpClientFactory.CreateHttpClient();

                Console.WriteLine($"Token requested with: ");
                Console.WriteLine($"Salt: {Convert.ToHexString(salt)}");
                Console.WriteLine($"Hash: {hash}");

                var authRequest = new { Pass = hash, Salt = salt };
                var content = new StringContent(JsonSerializer.Serialize(authRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(_apiUrl + "api/Auth/GetToken", content);

                // Handle response
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

                if (tokenResponse?.Token != null)
                {
                    Console.WriteLine($"Token obtained with success:\r\n{tokenResponse.Token}");
                    return tokenResponse.Token;
                }
                else
                {
                    throw new Exception("Null token.");
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
        private string? GetToken()
        {
            string[] envVars = GetEnvVariable().Split(";");
            if (envVars.Length > 1)
            {
                return envVars[1];
            }
            else
            {
                return null;
            }
        }
        private void StoreToken(string value)
        {
            string env = GetEnvVariable();
            env = env + ";" + value;
            Environment.SetEnvironmentVariable(_envName, env, EnvironmentVariableTarget.User);
        }
        private string GetEnvVariable()
        {
            Console.WriteLine($"Checking process for env var: {_envName}");
            string? value = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"Var {_envName} not found in process', checking User's");
                value = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.User);
            }
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"Var {_envName} not found in User's, checking Machine's");
                value = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.Machine);
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new NullReferenceException("Could not find API key.");
            }

            return value;
        }
        private string GetPasswordHash(out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(128 / 8);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(GetEnvVariable().Split(";")[0]))
            {
                Salt = salt,
                DegreeOfParallelism = 2,
                Iterations = 4,
                MemorySize = 512 * 512
            };
            byte[] hash = argon2.GetBytes(64);

            return Convert.ToBase64String(hash);
        }
    }
}