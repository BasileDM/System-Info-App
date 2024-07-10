namespace SystemInfoClient.Utilities
{
    public class ConsoleUtils
    {
        public static void WriteColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void LogTokenRequest()
        {
            Console.WriteLine();
            WriteColored("Requesting new token...", ConsoleColor.Yellow);
        }
        public static void LogMachineRequest()
        {
            Console.WriteLine();
            WriteColored("Sending machine info...", ConsoleColor.Yellow);
        }

        public static void LogAuthorizationError(HttpResponseMessage response)
        {
            WriteColored("Authorization error.", ConsoleColor.Red);
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
                WriteColored($"Machine successfully created.", ConsoleColor.Green);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                WriteColored($"Machine successfully updated.", ConsoleColor.DarkYellow);
            }

            Console.WriteLine($"Code: {response.StatusCode}.");

            if (response.Headers.Date.HasValue)
            {
                var utcDate = response.Headers.Date.Value;
                var localDate = utcDate.ToLocalTime();
                Console.WriteLine($"Local time: {localDate}.");
            }
            else
            {
                Console.WriteLine("Local time: No time header was provided.");
            }

            if (response.Headers.Location != null && !string.IsNullOrEmpty(response.Headers.Location.ToString()))
            {
                Console.WriteLine($"Location: {response.Headers.Location}.");
            }
            else
            {
                Console.WriteLine("Location: No location for machine updates.");
            }
        }
    }
}
