using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Cryptography;

namespace SystemInfoApi.Services
{
    public class CryptoService
    {
        public static bool VerifyPassword(string hash, byte[] salt)
        {
            if (hash == GetPasswordHashFromSalt(salt))
            {
                return true; 
            } 
            else
            {
                return false;
            }
        }
        public static string GetPasswordHashFromSalt(byte[] salt)
        {
            string pass = "PlaceholderPass123456789@test";

            var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(
                pass,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32);

            return Convert.ToBase64String(pbkdf2);
        }
    }
}
