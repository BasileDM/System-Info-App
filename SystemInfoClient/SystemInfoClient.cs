using System.Runtime.Versioning;
using SystemInfoClient.Classes;
using SystemInfoClient.Classes.System;
using SystemInfoClient.Services;

namespace SystemInfoClient
{
    [SupportedOSPlatform("windows")]
    public class SystemInfoClient
    {
        public static async Task Main()
        {
            try
            {
                Settings settings = Settings.GetInstance();
                EnvVariable envVariable = new("SysInfoApp", "54a7dV4o87.");
                SecurityService securityService = new(settings.ApiUrl, envVariable);
                MachineService machineService = new(settings.ApiUrl, securityService);

                // Create full machine with CustomerId, drives, os and apps info
                MachineClass machine = new(settings);

                // Fetch JWT token
                JwtToken token = await securityService.GetTokenAsync();

                // Send machine info to API route
                HttpResponseMessage response = await machineService.SendMachineInfoAsync(machine, token.GetString());

                // Handle API response
                await machineService.HandleResponseAsync(response, machine, settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message + ex);
            }
        }
    }
}