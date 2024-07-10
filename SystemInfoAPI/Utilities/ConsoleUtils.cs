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

        public static void LogMachineCreationRequest()
        {
            Console.WriteLine();
            WriteColored("New machine creation request...", ConsoleColor.Yellow);
        }
        public static void LogUpdateRequest(int machineId)
        {
            Console.WriteLine();
            WriteColored($"Issuing request to update a machine with ID: {machineId}...", ConsoleColor.Yellow);
        }
        public static void LogAuthRequestInfo(AuthRequest request, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteColored($"New token requested from: {connectionInfo.RemoteIpAddress?.ToString()} ...", ConsoleColor.Yellow);
            Console.WriteLine($"Request Content:");
            Console.WriteLine($"Full hash: {request.Pass}");
        }
        public static void LogGetMachineByIdRequest(int machineId)
        {
            Console.WriteLine();
            WriteColored($"Issuing request to get a machine by ID. Id: {machineId}", ConsoleColor.Yellow);
        }

        public static void LogMachineCreation(RouteValueDictionary? routeValues, MachineModel newMachine, IUrlHelper Url)
        {
            Console.WriteLine();
            WriteColored("A new machine has been created in the database.", ConsoleColor.Green);
            Console.WriteLine($"Time: {DateTime.Now.ToLocalTime()}");
            Console.WriteLine($"Customer ID: {newMachine.CustomerId}");
            Console.WriteLine($"Machine ID: {newMachine.Id}");
            Console.WriteLine($"Machine name: {newMachine.Name}");
            Console.WriteLine($"Drives amount: {newMachine.Drives.Count}");
            string? location = Url.Action(nameof(MachinesController.GetById), new { machineId = routeValues["machineId"] });
            Console.WriteLine($"Location: {location}");
        }
        public static void LogAppCreation(string appName, int appId, int appDriveId)
        {
            WriteColored($"Creating new app relation '{appName}' (id {appId}) on drive {appDriveId}.", ConsoleColor.Green);
        }

        public static void LogMachineUpdate(MachineModel machine)
        {
            WriteColored($"Machine {machine.Id} has been updated.", ConsoleColor.DarkYellow);
        }
    }
}
