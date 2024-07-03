using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using SystemInfoApi.Controllers;

namespace SystemInfoApi.Services
{
    public class AuthenticationService
    {
        public static bool VerifyPassword(string pass, string providedHash, byte[] providedSalt)
        {
            try
            {
                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(pass))
                {
                    Salt = providedSalt,
                    DegreeOfParallelism = 2,
                    Iterations = 4,
                    MemorySize = 512 * 512,
                };

                byte[] hash = argon2.GetBytes(64);
                string computedHash = Convert.ToBase64String(hash);

                if (providedHash == computedHash)
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
        public static string GenerateJwtToken(string secret, string issuer, int expirationTime)
        {
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var SecurityToken = new JwtSecurityToken(
                    issuer: issuer,
                    expires: DateTime.UtcNow.AddSeconds(expirationTime),
                    signingCredentials: credentials);

                Console.WriteLine($"Encoding token content:\r\n{SecurityToken}");
                string encodedToken = new JwtSecurityTokenHandler().WriteToken(SecurityToken);
                Console.WriteLine($"Sending encoded token:\r\n{encodedToken}");

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
        public static int ValidateExpirationTime(string? time)
        {
            if (!string.IsNullOrEmpty(time) && Int32.TryParse(time, out int parsedTime)) 
            {
                return parsedTime;
            }
            else
            {
                throw new ArgumentException("Invalid token expiration time in appsettings.json");
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
        public static string ValidateIssuer(string? issuer)
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
