using System.Security.Cryptography;

namespace SystemInfoClient.Services
{
    internal class SecurityService
    {
        public static string GetPasswordHash(out byte[] salt)
        {
            string pass = "Your-API-Password-123456789@test";

            salt = RandomNumberGenerator.GetBytes(128 / 8);

            var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(
                pass,
                salt,
                600000,
                HashAlgorithmName.SHA256,
                64);

            return Convert.ToBase64String(pbkdf2);
        }
    }
}
