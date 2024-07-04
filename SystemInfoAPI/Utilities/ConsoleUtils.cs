using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Controllers;
using SystemInfoApi.Models;

namespace SystemInfoApi.Utilities
{
    public class ConsoleUtils
    {
        public static void WriteColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void LogCreationInfo(RouteValueDictionary? routeValues, MachineModel newMachine, IUrlHelper Url)
        {
            string? location = Url.Action(nameof(MachinesController.GetById), new { machineId = routeValues["machineId"] });
            Console.WriteLine();
            Console.WriteLine("A new machine has been created in the database.");
            Console.WriteLine($"Time: {DateTime.Now.ToLocalTime()}");
            Console.WriteLine($"Customer ID: {newMachine.CustomerId}");
            Console.WriteLine($"Machine ID: {newMachine.Id}");
            Console.WriteLine($"Machine name: {newMachine.Name}");
            Console.WriteLine($"Drives amount: {newMachine.Drives.Count}");
            Console.WriteLine($"Location: {location}");
        }
        public static void LogAuthRequestInfo(AuthRequest request, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            Console.WriteLine($"New token requested from: {connectionInfo.RemoteIpAddress?.ToString()}");
            Console.WriteLine($"Request Content: \r\nHash: {request.Pass}");
        }
    }
}
