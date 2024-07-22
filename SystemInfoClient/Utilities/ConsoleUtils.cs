using System.Diagnostics;
using SystemInfoClient.Classes;

namespace SystemInfoClient.Utilities
{
    internal class ConsoleUtils
    {
        // Master switch:
        // true: switches all logs to true
        // false: all logs to false
        // null: logs will keep the value provided in SetProperty(value).
        private readonly static bool? _logsMasterSwitch = null;
        private readonly static bool _logTokenString = SetProperty(true);
        public readonly static bool _logDecodingProcess = SetProperty(false);
        public readonly static bool _logEnvVariableSetting = SetProperty(true);
        private readonly static bool _logHashingProcess = SetProperty(true);
        private readonly static bool _logJsonSettingsContent = SetProperty(false);

        private static readonly Stopwatch _stopwatch = new();
        private static long _totalTime = 0;

        public readonly static ConsoleColor _requestColor = ConsoleColor.Yellow;
        public readonly static ConsoleColor _creationColor = ConsoleColor.Green;
        public readonly static ConsoleColor _updateColor = ConsoleColor.Green;
        public readonly static ConsoleColor _deletionColor = ConsoleColor.DarkRed;

        public readonly static ConsoleColor _successColor = ConsoleColor.Green;
        public readonly static ConsoleColor _warningColor = ConsoleColor.DarkYellow;
        public readonly static ConsoleColor _errorColor = ConsoleColor.Red;

        // UTILS
        private static bool SetProperty(bool value)
        {
            // Returns the set property value, or false if _disableAllLogs is true.
            return _logsMasterSwitch ?? value;
        }
        public static void WriteLColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void WriteColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        private static void LogElapsedTime(bool final = false)
        {
            _totalTime += _stopwatch.ElapsedMilliseconds;
            string elapsed = _stopwatch.ElapsedMilliseconds.ToString();
            if (final)
            {
                WriteLColored($@"Total execution time: {_totalTime}ms.", ConsoleColor.DarkGray);
                _stopwatch.Stop();
            }
            else
            {
                WriteLColored($@" ({elapsed}ms | {_totalTime}ms)", ConsoleColor.DarkGray);
                _stopwatch.Restart();
            }
        }
        public static void StartTimer()
        {
            _stopwatch.Start();
        }

        // LOGS
        // Sending requests logs
        public static void LogTokenRequest()
        {
            Console.WriteLine();
            WriteLColored("Requesting new token...", _requestColor);
        }
        public static void LogMachineRequest()
        {
            Console.WriteLine();
            WriteLColored("Sending machine info...", _requestColor);
        }

        // Response info logs
        public static void LogAuthorizationError(HttpResponseMessage response)
        {
            WriteColored("Authorization error.", _errorColor);
            LogElapsedTime();
            if (response.Headers.WwwAuthenticate.Count > 0)
            {
                var wwwAuthHeader = response.Headers.WwwAuthenticate.ToString();
                var errorDescriptionIndex = wwwAuthHeader.IndexOf("error_description=");

                if (errorDescriptionIndex != -1)
                {
                    var errorDescription = wwwAuthHeader.Substring(errorDescriptionIndex + "error_description=".Length);
                    Console.WriteLine($"Error Description: {errorDescription}");
                }
            }
            else
            {
                Console.WriteLine($"Unexpected authorization error.");
            }
        }
        public static void LogResponseDetails(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                WriteColored($"Machine successfully created.", _successColor);
                LogElapsedTime();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                WriteColored($"Machine successfully updated.", _updateColor);
                LogElapsedTime();
            }

            Console.WriteLine($"  Code: {response.StatusCode}.");

            if (response.Headers.Date.HasValue)
            {
                var utcDate = response.Headers.Date.Value;
                var localDate = utcDate.ToLocalTime();
                Console.WriteLine($"  Local time: {localDate}.");
            }
            else
            {
                Console.WriteLine("  Local time: No time header was provided.");
            }

            if (response.Headers.Location != null && !string.IsNullOrEmpty(response.Headers.Location.ToString()))
            {
                Console.WriteLine($"  Location: {response.Headers.Location}.");
            }
            else
            {
                Console.WriteLine("  Location: No location for machine updates.");
            }
        }
        public static void LogTokenReceptionSuccess(string token)
        {
            WriteColored($"Token obtained with success.", _successColor);
            LogElapsedTime();
            if (_logTokenString) Console.WriteLine(token);
        }

        // Misc logs
        public static void LogJsonFileRewrite(string newMachineId, string json)
        {
            Console.Write($"Settings.json file rewritten with MachineId : {newMachineId}");
            LogElapsedTime();

            if (!_logJsonSettingsContent) return;
            Console.WriteLine($"Json content :");
            Console.WriteLine(json);
        }
        public static void LogEnvTokenSuccess(JwtToken token)
        {
            WriteColored($"Token found.", _successColor);
            LogElapsedTime();
            if (_logTokenString) Console.WriteLine(token.GetString());
        }
        public static void LogEnvTokenExpired(JwtToken? token)
        {
            ConsoleUtils.WriteColored(
                token == null ? "Token not found." : "Token expired.", ConsoleUtils._errorColor);
            LogElapsedTime();
        }
        public static void LogEnvDecodingProcess(string decoded)
        {
            if (!_logDecodingProcess) return;
            Console.WriteLine($"Decoding env variable...");
            Console.WriteLine($"Outter flag found and removed. Decoded result:");
            Console.Write(decoded);
            LogElapsedTime();
        }
        public static void StartLogEnvVariableSetting()
        {
            if (!_logEnvVariableSetting) return;
            Console.WriteLine("Setting env variable...");
        }
        public static void StopLogEnvVariableSetting(bool success)
        {
            if (success == true)
            {
                if (!_logEnvVariableSetting) return;
                Console.Write("Env variable set.");
                LogElapsedTime();
            }
            else
            {
                WriteColored("Failed setting environment variable.", _errorColor);
                LogElapsedTime();
            }
        }
        public static void LogHashingProcess(string source, byte[] salt, byte[] hash, string concat)
        {
            if (!_logHashingProcess) return;
            Console.WriteLine($"Hashing string: {source}");
            Console.WriteLine($"Salt: {Convert.ToHexString(salt)}");
            Console.WriteLine($"Hash: {Convert.ToBase64String(hash)}");
            Console.Write($"Concat salt and hash: {concat}");
            LogElapsedTime();
        }
        public static void LogTotalExecutionTime()
        {
            Console.WriteLine();
            LogElapsedTime(true);
        }
    }
}
