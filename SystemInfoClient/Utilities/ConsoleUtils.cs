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
            Console.WriteLine();
            WriteColored($"Machine data sent successfully.", ConsoleColor.Green);
            Console.WriteLine($"Code: {response.StatusCode}.");
            Console.WriteLine($"Time: {response.Headers.Date}.");

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
