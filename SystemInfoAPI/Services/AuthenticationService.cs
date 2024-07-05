using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SystemInfoApi.Services
{
    public class AuthenticationService
    {
        public static bool VerifyPassword(string pass, string providedHash)
        {
            try
            {
                var (Hash, Salt) = DecodeSaltAndHash(providedHash, 16, 64);
                string providedPass = Hash;
                byte[] providedSalt = Salt;
                Console.WriteLine($"Provided pass hash: {providedPass}");
                Console.WriteLine($"Provided salt: {Convert.ToHexString(providedSalt)}");

                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(pass))
                {
                    Salt = providedSalt,
                    DegreeOfParallelism = 2,
                    Iterations = 4,
                    MemorySize = 512 * 512,
                };

                byte[] hash = argon2.GetBytes(64);
                string computedPass = Convert.ToBase64String(hash);

                if (SecureCompare(providedPass, computedPass))
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
        private static bool SecureCompare(string a, string b)
        {
            if (a == null || b == null) return false;

            int diff = a.Length ^ b.Length;
            int maxLength = Math.Max(a.Length, b.Length);

            for (int i = 0; i < maxLength; i++)
            {
                char charA = i < a.Length ? a[i] : '\0';
                char charB = i < b.Length ? b[i] : '\0';
                diff |= charA ^ charB;
            }
            return diff == 0;
        }
        public static (string Hash, byte[] Salt) DecodeSaltAndHash(string saltAndHashBase64, int saltLength, int hashLength)
        {
            // Decode the base64 string to get the combined byte array
            byte[] saltAndHash = Convert.FromBase64String(saltAndHashBase64);

            // Extract the salt from the combined byte array
            byte[] salt = new byte[saltLength];
            Buffer.BlockCopy(saltAndHash, 0, salt, 0, salt.Length);

            // Extract the hash from the combined byte array
            byte[] hash = new byte[hashLength];
            Buffer.BlockCopy(saltAndHash, salt.Length, hash, 0, hash.Length);

            // Convert the hash to a base64 string
            string hashBase64 = Convert.ToBase64String(hash);

            return (hashBase64, salt);
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
    }
}
