using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.SqlClient;
using SystemInfoApi.Controllers;
using SystemInfoApi.Models;

namespace SystemInfoApi.Utilities
{
    public class ConsoleUtils
    {
        // UTILS
        public static void WriteColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        private static string GetExecutionTimeString(DateTime startTime)
        {
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            if (elapsed < 100)
            {
                return $"{(int)elapsed}ms";
            }
            else
            {
                var elapsedSeconds = elapsed / 1000;
                return $"{Math.Truncate(elapsedSeconds * 1000) / 1000} second(s)";
            }
        }
        private static void LogOriginIp(ConnectionInfo connectionInfo)
        {
            var ipv4 = connectionInfo.RemoteIpAddress?.MapToIPv4().ToString();
            var ipv6 = connectionInfo.RemoteIpAddress?.MapToIPv6().ToString();
            Console.WriteLine($"Origin: {ipv4} | {ipv6}");
        }

        // LOGS
        // Requests
        public static void LogMachineCreationRequest(ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteColored("New machine creation request...", ConsoleColor.Yellow);
            LogOriginIp(connectionInfo);
        }
        public static void LogUpdateRequest(int machineId, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteColored($"Issuing request to update a machine with ID: {machineId}...", ConsoleColor.Yellow);
            LogOriginIp(connectionInfo);
        }
        public static void LogAuthRequest(AuthRequest request, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteColored($"Issuing new token request...", ConsoleColor.Yellow);
            LogOriginIp(connectionInfo);
            Console.WriteLine($"Request Content:");
            Console.WriteLine($"Full hash: {request.Pass}");
        }
        public static void LogGetMachineByIdRequest(int machineId, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteColored($"Issuing request to get a machine by ID. Id: {machineId}", ConsoleColor.Yellow);
            LogOriginIp(connectionInfo);
        }
        // Create
        public static void LogMachineCreation(RouteValueDictionary? routeValues, MachineModel newMachine, IUrlHelper Url, DateTime startTime)
        {
            string totalTime = GetExecutionTimeString(startTime);

            Console.WriteLine();
            WriteColored($"Machine {newMachine.Id} has been created in {totalTime}.", ConsoleColor.Green);
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
        // Update
        public static void LogMachineUpdate(MachineModel machine, DateTime startTime)
        {
            string totalTime = GetExecutionTimeString(startTime);
            WriteColored($"Machine {machine.Id} has been updated in {totalTime}.", ConsoleColor.DarkYellow);
        }
        // Misc
        public static void LogTransactionStats(SqlConnection connection)
        {
            var stats = connection.RetrieveStatistics();
            Console.WriteLine("Transaction stats:");
            foreach (DictionaryEntry stat in stats)
            {
                Console.WriteLine($@"  {stat.Key} : {stat.Value}");
            }
        }
    }
}
