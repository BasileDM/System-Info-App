using System.Security.Cryptography;

namespace SystemInfoClient.Services
{
    internal class SecurityService
    {
        public static string GetPasswordHash(out byte[] salt)
        {
            // Get the API password from a user env variable
            string pass = Environment.GetEnvironmentVariable("SysInfoPass", EnvironmentVariableTarget.User) ??
                throw new NullReferenceException("The env variable 'SysInfoPass' could not be found");

            salt = RandomNumberGenerator.GetBytes(128 / 8);

            var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(
                pass,
                salt,
                1019358,
                HashAlgorithmName.SHA512,
                64);

            return Convert.ToBase64String(pbkdf2);
        }
    }
}
