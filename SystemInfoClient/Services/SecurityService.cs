using Konscious.Security.Cryptography;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemInfoApi.Utilities;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class SecurityService
    {
        private readonly string _apiUrl;
        private readonly string _envName = "SysInfoApp";
        private readonly string _flag = "enc.";

        public SecurityService(string apiUrl)
        {
            _apiUrl = apiUrl ?? throw new Exception("Invalid API URL in settings.json");
        }

        public async Task<string> GetOrRequestTokenAsync()
        {
            string? token = GetTokenFromEnv();
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
                ConsoleUtils.WriteColored("Requesting new token...", ConsoleColor.Yellow);
                var hash = GetHashedPassFromEnv();
                Console.WriteLine($"Request token method, GetHash returned: {hash}");

                // Prepare and send request
                HttpClient client = HttpClientFactory.CreateHttpClient();

                Console.WriteLine($"Token requested with: ");
                //Console.WriteLine($"Salt: {Convert.ToHexString(salt)}");
                Console.WriteLine($"Hash: {hash}");

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
        private string? GetTokenFromEnv()
        {
            string envValue = GetEnvVariableValue();
            Console.WriteLine($"Get token method: Decoded env value: {envValue}");
            string[] splitValues = envValue.Split(";");

            if (splitValues.Length > 1)
            {
                Console.WriteLine($"Token found: {splitValues[1]}");
                return splitValues[1];
            }
            else
            {
                Console.WriteLine($"Token not found");
                return null;
            }
        }
        private void StoreToken(string token)
        {
            string pass = GetHashedPassFromEnv();
            Console.WriteLine($"Store token method, Pass: {pass}");
            string newValue = pass + ";" + token;
            Console.WriteLine($"New value with token: {newValue}");
            string encoded = EncodeString(newValue);
            Console.WriteLine($"Encoded: {encoded}");
            Environment.SetEnvironmentVariable(_envName, encoded, EnvironmentVariableTarget.User);
        }
        private string GetEnvVariableValue()
        {
            Console.WriteLine($"Getting env raw getEnvVariable() method.");
            string? base64 = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(base64))
            {
                Console.WriteLine("Env not found in USERS, checking MACHINE");
                base64 = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.Machine);
            }
            if (string.IsNullOrEmpty(base64))
            {
                throw new NullReferenceException("Could not find API key.");
            }

            Console.WriteLine($"Raw : {base64}");
            string decoded = DecodeString(base64, out bool wasDecoded);
            if (wasDecoded)
            {
                Console.WriteLine("GetEnvValue method returned the decoded env variable.");
                Console.WriteLine($"{decoded}");
                return decoded;
            }
            else // If the flag is not found then the password is not yet hashed
            {
                string hash = HashString(base64);
                string encoded = EncodeString(hash);
                Environment.SetEnvironmentVariable(_envName, encoded, EnvironmentVariableTarget.User);

                Console.WriteLine("GetEnvValue method returned the hash alone.");
                Console.WriteLine($"{hash}");
                return hash;
            }
        }
        private string GetHashedPassFromEnv()
        {
            string envValue = GetEnvVariableValue();
            string[] splitValues = envValue.Split(";");

            if (splitValues.Length > 1 )
            {
                string pass = splitValues[0];
                return pass;
            } 
            else
            {
                return envValue;
            }

        }
        public string EncodeString(string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            string base64 = Convert.ToBase64String(bytes);
            string flagged = _flag + base64;
            return flagged;
        }
        public string DecodeString(string encodedString, out bool wasDecoded)
        {
            Console.WriteLine($"Decoding string: {encodedString}");
            if (encodedString.Contains(_flag))
            {
                wasDecoded = true;
                string unflagged = encodedString.Replace(_flag, "");
                Console.WriteLine($"Flag found and removed :  {unflagged}");
                byte[] bytes = Convert.FromBase64String(unflagged);
                string decoded = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"Decoded {decoded}");
                return decoded;
            }
            else
            {
                wasDecoded = false;
                Console.WriteLine($"Flag not found returning string without decoding");
                return encodedString;
            }
        }
        private static string HashString(string source)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(source))
            {
                Salt = salt,
                DegreeOfParallelism = 2,
                Iterations = 4,
                MemorySize = 512 * 512
            };
            byte[] hash = argon2.GetBytes(64);

            // Concatenate the salt and hash
            byte[] saltAndHash = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, saltAndHash, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, saltAndHash, salt.Length, hash.Length);

            return Convert.ToBase64String(saltAndHash);
        }
    }
    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}