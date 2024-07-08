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
        private static Settings _settings = null!;
        private static MachineClass _machine = null!;
        private static EnvVariable _env = null!;
        private static SecurityService _securityService = null!;
        private static MachineService _machineService = null!;

        public static async Task Main()
        {
            try
            {
                // Instanciate required objects and services
                _settings = Settings.GetInstance();
                _env = new("SysInfoApp");
                _machine = new(_settings);
                _securityService = new(_settings.ApiUrl, _env);
                _machineService = new(_settings.ApiUrl, _machine);

                // Fetch JWT token
                JwtToken token = await _securityService.GetTokenAsync();

                // Send machine info to API route
                HttpResponseMessage response = await _machineService.SendMachineInfoAsync(token.GetString());

                // Handle API response
                await HandleResponseAsync(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
        }

        private static async Task HandleResponseAsync(HttpResponseMessage response)
        {
            try
            {
                switch (response.StatusCode)
                {
                    // Machine creation
                    case HttpStatusCode.Created when response.Headers.Location != null:
                        string? newMachineId = MachineService.GetMachineIdFromResponse(response);
                        ConsoleUtils.LogResponseDetails(response);
                        _settings.RewriteFileWithId(newMachineId);
                        break;

                    // Machine update
                    case HttpStatusCode.OK:
                        ConsoleUtils.LogResponseDetails(response);
                        break;

                    // Unauthorized
                    case HttpStatusCode.Unauthorized:
                        ConsoleUtils.LogAuthorizationError(response);

                        // Try to obtain a new token
                        JwtToken newToken = await _securityService.RequestTokenAsync();

                        // Send machine info with new token
                        HttpResponseMessage retryResponse = await _machineService.SendMachineInfoAsync(newToken.GetString());

                        retryResponse.EnsureSuccessStatusCode();
                        ConsoleUtils.LogResponseDetails(retryResponse);

                        newMachineId = MachineService.GetMachineIdFromResponse(retryResponse);
                        _settings.RewriteFileWithId(newMachineId);
                        break;

                    // Generic
                    default:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine();
                        Console.WriteLine($"{response.ReasonPhrase}: {errorContent}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during API response handling: " + ex.Message);
            }
        }
    }
}