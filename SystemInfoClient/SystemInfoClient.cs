using System.Net;
using System.Runtime.Versioning;
using SystemInfoClient.Classes;
using SystemInfoClient.Classes.System;
using SystemInfoClient.Services;
using SystemInfoClient.Utilities;

namespace SystemInfoApi
{
    [SupportedOSPlatform("windows")]
    public class SystemInfoClient
    {
        public static async Task Main()
        {
            try
            {
                // Timers for performance monitoring
                ConsoleUtils.StartWatch();

                // Instanciate required objects and services
                EnvVariable env = new("SysInfoApp");
                Settings settings = Settings.GetInstance();

                MachineClass machine = new(settings);

                MachineService machineService = new(settings.ApiUrl, machine);
                SecurityService security = new(settings.ApiUrl, env);

                // Fetch token
                JwtToken token = await security.GetTokenAsync();

                // Send machine info to API
                HttpResponseMessage response = await machineService.SendMachineInfoAsync(token.GetString());

                // Handle API response
                await HandleResponseAsync(response, settings, security, machineService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
        }

        private static async Task HandleResponseAsync(
            HttpResponseMessage response, Settings settings, SecurityService security, MachineService machineService)
        {
            try
            {
                switch (response.StatusCode)
                {
                    // Machine creation
                    case HttpStatusCode.Created when response.Headers.Location != null:
                        string? newMachineId = MachineService.GetMachineIdFromResponse(response);
                        ConsoleUtils.LogResponseDetails(response);
                        settings.RewriteFileWithId(newMachineId);
                        break;

                    // Machine update
                    case HttpStatusCode.OK:
                        ConsoleUtils.LogResponseDetails(response);
                        break;

                    // Unauthorized
                    case HttpStatusCode.Unauthorized:
                        ConsoleUtils.LogAuthorizationError(response);

                        // Try to obtain a new token
                        JwtToken newToken = await security.RequestTokenAsync();

                        // Send machine info with new token
                        HttpResponseMessage retryResponse = await machineService.SendMachineInfoAsync(newToken.GetString());

                        retryResponse.EnsureSuccessStatusCode();
                        ConsoleUtils.LogResponseDetails(retryResponse);

                        newMachineId = MachineService.GetMachineIdFromResponse(retryResponse);
                        settings.RewriteFileWithId(newMachineId);
                        break;

                    // Generic
                    default:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine();
                        Console.WriteLine($"{response.ReasonPhrase}: {errorContent}");
                        break;
                }
                ConsoleUtils.LogTotalExecutionTime();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling the API response: " + ex.Message);
            }
        }
    }
}