using Konscious.Security.Cryptography;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemInfoClient.Classes;
using SystemInfoClient.Utilities;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class SecurityService
    {
        private readonly string _apiUrl;
        private readonly EnvVariable _envVariable;

        public SecurityService(string apiUrl, EnvVariable envVariable)
        {
            _apiUrl = apiUrl ?? throw new Exception("Invalid API URL in settings.json");
            _envVariable = envVariable;
        }

        public async Task<JwtToken> GetTokenAsync()
        {
            JwtToken? token = _envVariable.Token;
            if (token == null)
            {
                Console.WriteLine("Token not found.");
                token = await RequestTokenAsync();
            }
            else
            {
                Console.WriteLine($"Token found: \r\n{token.GetString()}");
            }

            return token;
        }
        public async Task<JwtToken> RequestTokenAsync()
        {
            try
            {
                ConsoleUtils.WriteColored("Requesting new token...", ConsoleColor.Yellow);
                string hash = _envVariable.Hash;

                // Prepare and send request
                HttpClient client = HttpClientFactory.CreateHttpClient();

                var authRequest = new { Pass = hash };
                var content = new StringContent(JsonSerializer.Serialize(authRequest), Encoding.UTF8, "application/json");
                string route = $"{_apiUrl}api/Auth/GetToken";

                HttpResponseMessage response = await client.PostAsync(route, content);

                // Handle response
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

                if (tokenResponse?.Token != null)
                {
                    ConsoleUtils.WriteColored($"Token obtained with success: ", ConsoleColor.Green);
                    Console.WriteLine(tokenResponse.Token);
                    JwtToken token = JwtToken.GetInstance(tokenResponse.Token);
                    _envVariable.Token = token;
                    return token;
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
        public static string HashString(string source)
        {
            Console.WriteLine($"Hashing string: {source}");

            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            Console.WriteLine($"Salt: {Convert.ToHexString(salt)}");

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(source))
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                Iterations = 4,
                MemorySize = 512 * 512
            };
            byte[] hash = argon2.GetBytes(64);
            Console.WriteLine($"Hash: {Convert.ToBase64String(hash)}");

            // Concatenate the salt and hash
            byte[] saltAndHash = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, saltAndHash, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, saltAndHash, salt.Length, hash.Length);

            string hashSaltConcatString = Convert.ToBase64String(saltAndHash);
            Console.WriteLine($"Concat salt and hash: {hashSaltConcatString}");
            return hashSaltConcatString;
        }
    }

    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}