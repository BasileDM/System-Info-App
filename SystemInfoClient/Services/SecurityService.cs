using Konscious.Security.Cryptography;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SystemInfoApi.Utilities;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class SecurityService
    {
        private readonly string _apiUrl;
        private readonly string _envName = "SysInfoApp";
        private readonly byte[] _key = Convert.FromBase64String("vgIBk3E0UTqRlCog5OqyxNAvHU0kpue9gxTbMt24n1g=");
        private readonly byte[] _iv = Convert.FromHexString("ee62dd0adcbf5a6a");
        private readonly string _flag = "enc.";

        public SecurityService(string apiUrl)
        {
            _apiUrl = apiUrl ?? throw new Exception("Invalid API URL in settings.json");
        }

        public async Task<string> GetOrRequestTokenAsync()
        {
            string? token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                token = await RequestTokenAsync();
            }

            return token;
        }
        public async Task<string> RequestTokenAsync()
        {
            try
            {
                Logger.WriteColored("Requesting new token...", ConsoleColor.Yellow);
                var hash = GetPasswordHash(out byte[] salt);

                // Prepare and send request
                HttpClient client = HttpClientFactory.CreateHttpClient();

                Console.WriteLine($"Token requested with: ");
                Console.WriteLine($"Salt: {Convert.ToHexString(salt)}");
                Console.WriteLine($"Hash: {hash}");

                var authRequest = new { Pass = hash, Salt = salt };
                var content = new StringContent(JsonSerializer.Serialize(authRequest), Encoding.UTF8, "application/json");
                string route = $"{_apiUrl}api/Auth/GetToken";

                HttpResponseMessage response = await client.PostAsync(route, content);

                // Handle response
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

                if (tokenResponse?.Token != null)
                {
                    Logger.WriteColored($"Token obtained with success: ", ConsoleColor.Green);
                    Console.WriteLine(tokenResponse.Token);
                    StoreToken(tokenResponse.Token);
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
            string envVar = GetEnvVariableValue();
            if (envVar.Contains(_flag))
            {
                string decoded = DecodeString(envVar);
                string[] envVars = decoded.Split(";");
                if (envVars.Length > 1)
                {
                    return envVars[1];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return envVar;
            }
        }
        private void StoreToken(string value)
        {
            string envValue = GetEnvVariableValue();
            string pass = envValue.Split(";")[0];
            envValue = pass + ";" + value;
            string encoded = EncodeString(envValue);
            Environment.SetEnvironmentVariable(_envName, $"{_flag}{encoded}", EnvironmentVariableTarget.User);
        }
        private string GetEnvVariableValue()
        {
            string? value = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(value))
            {
                value = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.User);
            }
            if (string.IsNullOrEmpty(value))
            {
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

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(GetEnvVariableValue().Split(";")[0]))
            {
                Salt = salt,
                DegreeOfParallelism = 2,
                Iterations = 4,
                MemorySize = 512 * 512
            };
            byte[] hash = argon2.GetBytes(64);

            return Convert.ToBase64String(hash);
        }
        public string EncodeString(string source)
        {
            Console.WriteLine($"Encoding: {source}");
            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using MemoryStream memoryStream = new();

            using (CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using StreamWriter sw = new(cryptoStream);
                sw.Write(source);
            }

            byte[] encrypted = memoryStream.ToArray();
            return Convert.ToBase64String(encrypted);
        }
        public string DecodeString(string encodedString)
        {
            Console.WriteLine($"Decoding: {encodedString}");
            byte[] buffer = Convert.FromBase64String(encodedString);

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using MemoryStream memoryStream = new(buffer);

            using (CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using StreamReader streamReader = new(cryptoStream);
                string decrypted = streamReader.ReadToEnd();
                return decrypted;
            }

        }
    }
}