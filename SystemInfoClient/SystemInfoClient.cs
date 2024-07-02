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
                NetworkService net = new(settings);
                SecurityService securityService = new(net);

                // Create full machine with CustomerId, drives, os and apps info
                MachineClass machine = new(settings);
                machine.LogJson();

                // Fetch JWT token
                string token = await securityService.GetOrRequestTokenAsync();
                //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3Mjk5MTgxMjUsImlzcyI6IlN5c3RlbUluZm9BcGkifQ.176_JjhiMPFLZLuSSwFexS6BHtBVNYXCuHNT4LvVpWk";

                // POST machine to API route
                HttpResponseMessage response = await net.SendMachineInfo(machine, token);

                // Handle API response
                if (await NetworkService.IsResponseOk(response))
                {
                    string newMachineId = GetMachineIdFromResponse(response);

                    if (newMachineId != settings.ParsedMachineId.ToString())
                        settings.RewriteFileWithId(newMachineId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
        }
        private static string GetMachineIdFromResponse(HttpResponseMessage response)
        {
            // Parse the last element of the Location header in the response to get the new machine ID
            string machineId;
            if (response.Headers.Location != null)
            {
                machineId = response.Headers.Location.Segments.Last();
            }
            else
            {
                throw new InvalidDataException("Invalid API response's location header");
            }

            if (Int32.TryParse(machineId, out int parsedMachineId) && parsedMachineId > 0)
            {
                return machineId;
            }
            else
            {
                throw new InvalidDataException("The machine ID sent by the API was invalid.");
            }
        }
    }
}