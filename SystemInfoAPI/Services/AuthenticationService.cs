using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using SystemInfoApi.Controllers;

namespace SystemInfoApi.Services
{
    public class AuthenticationService
    {
        public static bool VerifyPassword(string apiPass, string sentPassHash, byte[] sentSalt)
        {
            try
            {
                var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(
                    apiPass,
                    sentSalt,
                    600000,
                    HashAlgorithmName.SHA256,
                    64);

                string apiPassHash = Convert.ToBase64String(pbkdf2);

                if (sentPassHash == apiPassHash)
                {
                    Console.WriteLine("Password is valid.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid password.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying password: {ex.Message}");
                throw new Exception("The password could not be verified.", ex);
            }
        }
        public static string GenerateJwtToken(IConfiguration config)
        {
            try
            {
                Console.WriteLine("Generating token...");
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]));
                var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var SecurityToken = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    expires: DateTime.UtcNow.AddMilliseconds(120),
                    signingCredentials: credentials);

                Console.WriteLine($"Encoding token: {SecurityToken}");
                string encodedToken = new JwtSecurityTokenHandler().WriteToken(SecurityToken);
                Console.WriteLine($"Sending encoded token: {encodedToken}");

                return encodedToken;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Configuration error: {ex.Message}");
                throw new Exception("JWT configuration is invalid. Please check the JWT secret and issuer in the configuration.", ex);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Argument error: {ex.Message}");
                throw new Exception("An argument error occurred while generating the JWT token.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred while generating the JWT token.", ex);
            }
        }
        public static string ValidateSecret(string? secret)
        {
            if (secret == null || secret.Length < 33)
            {
                int keyLength = secret == null ? 0 : secret.Length;
                throw new ApplicationException(
                    $"Invalid secret key in appsettings.json, key length must be at least 33 characters and was {keyLength}");
            }
            else
            {
                return secret;
            }
        }
        public static string ValidaterIssuer(string? issuer)
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
        public static string ValidateApiPass(string? apiPass)
        {
            if (apiPass != null && apiPass.Length >= 20)
            {
                return apiPass;
            }
            else
            {
                throw new InvalidDataException("Invalid API password in appsettings.json, the password must be at least 20 characters.");
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
