using Konscious.Security.Cryptography;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class SecurityService
    {
        private readonly NetworkService _net;
        private readonly string _envName = "SysInfoApp";

        public SecurityService(NetworkService networkService)
        {
            _net = networkService;
        }

        public async Task<string> GetOrRequestTokenAsync()
        {
            string? token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                var hash = GetPasswordHash(out byte[] salt);
                token = await _net.SendTokenRequest(hash, salt);
                StoreToken(token);
            }

            return token;
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
                throw new NullReferenceException("Error, could not find API key.");
            }

            return value;
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
    }
}