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
                // Instanciate needed objects
                SettingsClass settings = SettingsClass.GetInstance();
                SecurityService securityService = new(settings.ApiUrl);
                MachineService machineService = new(settings.ApiUrl, securityService);

                // Create full machine with CustomerId, drives, os and apps info
                MachineClass machine = new(settings);
                machine.LogJson();

                // Fetch JWT token
                string token = await securityService.GetOrRequestTokenAsync();
                

                // POST machine to API route
                HttpResponseMessage response = await machineService.SendMachineInfoAsync(machine, token);

                // Handle API response
                await machineService.HandleResponseAsync(response, machine, settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
        }
    }
}