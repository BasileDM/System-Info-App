using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace SystemInfoClient.Services
{
    internal class SecurityService
    {
        public static string GetPasswordHash(out byte[] salt)
        {
            // Get the API password from a user env variable
            string pass = Environment.GetEnvironmentVariable("SysInfoApp", EnvironmentVariableTarget.User) ??
                throw new NullReferenceException("Error, null API key.");

            salt = RandomNumberGenerator.GetBytes(128 / 8);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(pass))
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
