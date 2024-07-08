using System.Net.Http.Headers;
using System.Text;
using System.Runtime.Versioning;
using System.Net;
using SystemInfoClient.Utilities;
using SystemInfoClient.Classes.System;
using SystemInfoClient.Classes;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class MachineService
    {
        private readonly string _apiUrl;
        private readonly MachineClass Machine;

        public MachineService(string apiUrl, SecurityService securityService, MachineClass machine)
        {
            _apiUrl = apiUrl ?? throw new Exception("Invalid API URL in settings.json");
            Machine = machine;
        }

        public async Task<HttpResponseMessage> SendMachineInfoAsync(string token)
        {
            ConsoleUtils.WriteColored("Sending machine info...", ConsoleColor.Yellow);

            // Build HTTP Client and add authorization header with token
            HttpClient client = HttpClientFactory.CreateHttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize machine into JSON content, build route string, and send
            // If the machine ID is 0 it is a new machine
            var content = new StringContent(Machine.JsonSerialize(), Encoding.UTF8, "application/json");

            string route = Machine.Id == 0 ?
                $"{_apiUrl}api/Machines/Create" :
                $"{_apiUrl}api/Machines/Update/{Machine.Id}";

            return Machine.Id == 0 ?
                await client.PostAsync(route, content) :
                await client.PutAsync(route, content);
        }
        public static string? GetMachineIdFromResponse(HttpResponseMessage response)
        {
            // Parse the last element of the Location header in the response to get the new machine ID
            string machineId;
            if (response.Headers.Location != null)
            {
                machineId = response.Headers.Location.Segments.Last();
            }
            else
            {
                return null;
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
