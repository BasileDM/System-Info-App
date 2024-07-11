﻿using SystemInfoClient.Classes;

namespace SystemInfoClient.Utilities
{
    internal class ConsoleUtils
    {
        private readonly static bool? _logsMasterSwitch = true;

        private readonly static bool _logTokenString = SetProperty(true);
        public readonly static bool _logDecodingProcess = SetProperty(true);
        private readonly static bool _logHashingProcess = SetProperty(true);
        private readonly static bool _logJsonSettingsContent = SetProperty(true);

        public readonly static ConsoleColor _requestColor = ConsoleColor.Yellow;
        public readonly static ConsoleColor _creationColor = ConsoleColor.Green;
        public readonly static ConsoleColor _updateColor = ConsoleColor.DarkYellow;
        public readonly static ConsoleColor _deletionColor = ConsoleColor.DarkRed;

        public readonly static ConsoleColor _successColor = ConsoleColor.Green;
        public readonly static ConsoleColor _errorColor = ConsoleColor.Red;

        // UTILS
        private static bool SetProperty(bool value)
        {
            // Returns the set property value, or false if _disableAllLogs is true.
            return _logsMasterSwitch ?? value;
        }
        public static void WriteColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        private static string GetExecutionTimeString(DateTime startTime)
        {
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            if (elapsed < 100)
            {
                return $"{(int)elapsed}ms";
            }
            else
            {
                var elapsedSeconds = elapsed / 1000;
                return $"{Math.Truncate(elapsedSeconds * 1000) / 1000} second(s)";
            }
        }

        // LOGS
        // Sending requests logs
        public static void LogTokenRequest()
        {
            Console.WriteLine();
            WriteColored("Requesting new token...", _requestColor);
        }
        public static void LogMachineRequest()
        {
            Console.WriteLine();
            WriteColored("Sending machine info...", _requestColor);
        }

        // Response info logs
        public static void LogAuthorizationError(HttpResponseMessage response)
        {
            WriteColored("Authorization error.", _errorColor);
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
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                WriteColored($"Machine successfully updated.", _updateColor);
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
            if (_logTokenString) Console.WriteLine(token);
        }

        // Misc logs
        public static void LogJsonFileRewrite(string newMachineId, string json)
        {
            Console.WriteLine($"Settings.json file rewritten with MachineId : {newMachineId}");

            if (!_logJsonSettingsContent) return;
            Console.WriteLine($"Json content :");
            Console.WriteLine(json);
        }
        public static void LogEnvTokenSuccess(JwtToken token)
        {
            WriteColored($"Token found.", _successColor);
            if (_logTokenString) Console.WriteLine(token.GetString());
        }
        public static void LogHashingProcess(string source, byte[] salt, byte[] hash, string concat)
        {
            if (!_logHashingProcess) return;
            Console.WriteLine($"Hashing string: {source}");
            Console.WriteLine($"Salt: {Convert.ToHexString(salt)}");
            Console.WriteLine($"Hash: {Convert.ToBase64String(hash)}");
            Console.WriteLine($"Concat salt and hash: {concat}");
        }
        public static void LogEnvDecodingProcess(string decoded)
        {
            if (!_logDecodingProcess) return;
            Console.WriteLine($"Decoding env variable...");
            Console.WriteLine($"Outter flag found and removed. Decoded result:");
            Console.WriteLine(decoded);
        }
        public static void LogTotalExecutionTime(DateTime startTime)
        {
            Console.WriteLine();
            Console.WriteLine($"Total execution time: {GetExecutionTimeString(startTime)}");
        }
    }
}
