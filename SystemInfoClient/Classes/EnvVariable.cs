using System.Runtime.Versioning;
using System.Text;
using SystemInfoClient.Services;

namespace SystemInfoClient.Classes
{
    [SupportedOSPlatform("windows")]
    internal class EnvVariable
    {
        private readonly string _envName;
        private readonly string _flag;
        private string _decodedValue;

        public string Hash
        {
            get => _decodedValue.Split(";")[0];
            set
            {
                Console.WriteLine($"New hash value to set : {value}");
                string newValue = _decodedValue.Split(";").Length > 1 ? $"{value};{Token?.GetString()}" : $"{value}";
                SetDecodedValue(newValue);
            }
        }
        public JwtToken? Token
        {
            get
            {
                string[] splitValues = _decodedValue.Split(";");
                return splitValues.Length > 1 ? JwtToken.GetInstance(splitValues[1]) : null;
            }
            set
            {
                string newValue = $"{Hash};{value?.GetString()}";
                SetDecodedValue(newValue);
            }
        }

        public EnvVariable(string envName, string flag)
        {
            _envName = envName;
            _flag = flag;
            _decodedValue = GetDecodedValue();

            string decodedValue = _decodedValue;
            if (decodedValue.Split(';').Length > 1)
            {
                Token = JwtToken.GetInstance(decodedValue.Split(';')[1]);
            }
        }

        private void SetDecodedValue(string value)
        {
            _decodedValue = value;
            Environment.SetEnvironmentVariable(_envName, EncodeString(value), EnvironmentVariableTarget.User);
        }
        private string GetDecodedValue()
        {
            string base64 = Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.User) ??
                            Environment.GetEnvironmentVariable(_envName, EnvironmentVariableTarget.Machine) ??
                            throw new NullReferenceException("Could not find API key.");

            string decoded = DecodeStringIfFlagged(base64, out bool wasDecoded);
            if (wasDecoded)
            {
                return decoded;
            }
            else
            {
                string hash = SecurityService.HashString(base64);
                string encoded = EncodeString(hash);
                Environment.SetEnvironmentVariable(_envName, encoded, EnvironmentVariableTarget.User);
                return hash;
            }
        }
        private string EncodeString(string source)
        {
            string flaggedSource = _flag + source;
            byte[] bytes = Encoding.UTF8.GetBytes(flaggedSource);
            string base64 = Convert.ToBase64String(bytes);
            string flagged = _flag + base64;
            return flagged;
        }
        private string DecodeStringIfFlagged(string encodedString, out bool wasDecoded)
        {
            if (encodedString.StartsWith(_flag))
            {
                wasDecoded = true;
                string unflagged = encodedString.Substring(_flag.Length);
                Console.WriteLine($"Decoding env variable...");
                byte[] bytes;

                try
                {
                    bytes = Convert.FromBase64String(unflagged);
                }
                catch (FormatException)
                {
                    wasDecoded = false;
                    return encodedString;
                }

                string decoded = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"Flag found, removed, and string decoded:");
                Console.WriteLine(decoded + "\r\n");

                if (decoded.StartsWith(_flag))
                {
                    decoded = decoded.Substring(_flag.Length);
                    return decoded;
                }
                else
                {
                    wasDecoded = false;
                    return encodedString;
                }
            }
            else
            {
                wasDecoded = false;
                Console.WriteLine($"Flag not detected, not decoding.");
                return encodedString;
            }
        }
    }
}
