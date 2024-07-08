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
                Settings settings = Settings.GetInstance();

                // Create full machine with CustomerId, drives, os and apps info
                MachineClass machine = new(settings);

                EnvVariable envVariable = new("SysInfoApp");
                SecurityService securityService = new(settings.ApiUrl, envVariable);
                MachineService machineService = new(settings.ApiUrl, securityService, machine);


                // Fetch JWT token
                JwtToken token = await securityService.GetTokenAsync();

                // Send machine info to API route
                HttpResponseMessage response = await machineService.SendMachineInfoAsync(token.GetString());

                // Handle API response
                await HandleResponseAsync(response, settings, machineService, securityService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
        }
        private static async Task HandleResponseAsync(
            HttpResponseMessage response, Settings settings, MachineService machineService, SecurityService securityService)
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
                    JwtToken newToken = await securityService.RequestTokenAsync();

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
        }
    }
}