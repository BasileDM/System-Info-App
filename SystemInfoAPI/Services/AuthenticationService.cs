using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using SystemInfoApi.Controllers;

namespace SystemInfoApi.Services
{
    public class AuthenticationService
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
                172099,
                HashAlgorithmName.SHA256,
                64);

            return Convert.ToBase64String(pbkdf2);
        }
        public static string GenerateJwtToken(IConfiguration config)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var SecurityToken = new JwtSecurityToken(
                config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(SecurityToken);
        }
        public static string ValidateSecret(string secret)
        {
            if (secret == null || secret.Length < 33)
            {
                int keyLength = secret == null ? 0 : secret.Length;
                throw new ApplicationException(
                    $"Invalid secret key in appsettings.json, key length must be at least 33 characters and was {keyLength}");
            }
            return secret;
        }
        public static string ValidaterIssuer(string issuer)
        {
            if (issuer == null)
            {
                throw new Exception("Invalid issuer in appsettings.json.");
            }
            else
            {
                return issuer;
            }
        }
        public static void LogRequestInfo(AuthRequest request, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            Console.WriteLine($"New token requested from: {connectionInfo.RemoteIpAddress?.ToString()}");
            Console.WriteLine($"Request Content: \r\nHash: {request.Pass}, \r\nSalt: {Convert.ToHexString(request.Salt)}");
        }
    }
}
