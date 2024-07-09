using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemInfoClient.Classes
{
    internal class JwtToken
    {
        public JwtHeader? Header { get; private set; }
        public JwtPayload? Payload { get; private set; }
        public string? Signature { get; private set; }

        public static JwtToken? GetInstance(string jwt)
        {
            string[] parts = jwt.Split('.');

            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT token");
            }

            string headerJson = Base64UrlDecode(parts[0]);
            string payloadJson = Base64UrlDecode(parts[1]);
            string signature = parts[2];

            var header = JsonSerializer.Deserialize<JwtHeader>(headerJson);
            var payload = JsonSerializer.Deserialize<JwtPayload>(payloadJson);

            JwtToken token = new()
            {
                Header = header,
                Payload = payload,
                Signature = signature
            };

            return token;
        }
        private static string Base64UrlDecode(string base64Url)
        {
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(base64);
            string decoded = Encoding.UTF8.GetString(bytes);
            return decoded;
        }
        private static string Base64UrlEncode(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string base64 = Convert.ToBase64String(bytes);
            return base64.Replace('+', '-').Replace('/', '_').Replace("=", string.Empty);
        }
        public string GetString()
        {
            string headerJson = JsonSerializer.Serialize(Header);
            string payloadJson = JsonSerializer.Serialize(Payload);

            string encodedHeader = Base64UrlEncode(headerJson);
            string encodedPayload = Base64UrlEncode(payloadJson);

            return $"{encodedHeader}.{encodedPayload}.{Signature}";
        }
        public bool IsExpired()
        {
            if (Payload != null)
            {
                int currentUnixTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return Payload.Exp <= currentUnixTime;
            }
            else
            {
                return false;
            }
        }
    }

    internal class JwtHeader
    {
        [JsonPropertyName("alg")]
        public string Alg { get; set; }

        [JsonPropertyName("typ")]
        public string Typ { get; set; }
    }
    internal class JwtPayload
    {
        [JsonPropertyName("exp")]
        public int Exp { get; set; }

        [JsonPropertyName("iss")]
        public string Iss { get; set; }
    }
}
