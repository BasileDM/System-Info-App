using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using SystemInfoApi.Controllers;
using SystemInfoApi.Models;

namespace SystemInfoApi.Utilities
{
    public class ConsoleUtils
    {
        // Master switch:
        // true, switches all logs to true
        // false, all logs to false
        // null, logs will keep the value provided in SetProperty(value).
        private readonly static bool? _logsMasterSwitch = false;
        public readonly static bool _logTransactionNotice = SetProperty(true);
        private readonly static bool _logTransactionStats = SetProperty(true);
        private readonly static bool _logAuthRequestContent = SetProperty(false);
        private readonly static bool _logCreationDetails = SetProperty(true);

        private readonly static bool _logTokenContent = SetProperty(true);
        private readonly static bool _logEncodedToken = SetProperty(false);
        private readonly static bool _logHashSalt = SetProperty(false);

        private static Timer? _timer;
        private static Stopwatch? _stopwatch;

        public readonly static ConsoleColor _requestColor = ConsoleColor.Yellow;
        public readonly static ConsoleColor _creationColor = ConsoleColor.Green;
        public readonly static ConsoleColor _updateColor = ConsoleColor.DarkYellow;
        public readonly static ConsoleColor _deletionColor = ConsoleColor.DarkRed;

        public readonly static ConsoleColor _successColor = ConsoleColor.DarkGreen;
        public readonly static ConsoleColor _errorColor = ConsoleColor.Red;

        // UTILS
        private static bool SetProperty(bool value)
        {
            // Returns the set property value, or false if _disableAllLogs is true.
            return _logsMasterSwitch ?? value;
        }
        public static void WriteLineColored(string message, ConsoleColor color)
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

        // LOGS
        // Recieving requests logs
        public static void LogMachineCreationRequest(ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteLineColored("New machine creation request...", _requestColor);
            LogRequestIpOrigin(connectionInfo);
        }
        public static void LogUpdateRequest(int machineId, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteLineColored($"Issuing request to update machine {machineId}...", _requestColor);
            LogRequestIpOrigin(connectionInfo);
        }
        public static void LogAuthRequest(AuthRequest request, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteLineColored($"Issuing new token request...", _requestColor);
            LogRequestIpOrigin(connectionInfo);

            if (!_logAuthRequestContent) return;
            Console.WriteLine($"Request Content:");
            Console.WriteLine($"Full hash: {request.Pass}");
        }
        public static void LogGetMachineByIdRequest(int machineId, ConnectionInfo connectionInfo)
        {
            Console.WriteLine();
            WriteLineColored($"Issuing request to get a machine with Id '{machineId}'", _requestColor);
            LogRequestIpOrigin(connectionInfo);
        }
        
        // Sending requests logs
        public static void LogSendingToken(string encodedToken)
        {
            if (!_logEncodedToken) goto request;
            Console.WriteLine($"Encoded token:");
            Console.WriteLine(encodedToken);

        request:
            ConsoleUtils.WriteLineColored("Sending token...", _requestColor);
        }

        // Creation logs
        public static void LogMachineCreation(RouteValueDictionary? routeValues, MachineModel newMachine, IUrlHelper Url, DateTime startTime)
        {
            string totalTime = GetExecutionTimeString(startTime);

            WriteLineColored($"Machine {newMachine.Id} has been created in {totalTime}.", _creationColor);

            if (!_logCreationDetails) return;
            Console.WriteLine($@"  Time: {DateTime.Now.ToLocalTime()}");
            Console.WriteLine($@"  Customer ID: {newMachine.CustomerId}");
            Console.WriteLine($@"  Machine name: {newMachine.Name}");
            Console.WriteLine($@"  Drives amount: {newMachine.Drives.Count}");
            string? location = Url.Action(nameof(MachinesController.GetById), new { machineId = routeValues["machineId"] });
            Console.WriteLine($@"  Location: {location}");
        }
        public static void LogAppCreation(string appName, int appId, int appDriveId)
        {
            WriteLineColored($"Creating new app relation '{appName}' (id {appId}) on drive {appDriveId}.", _creationColor);
        }
        
        // Update logs
        public static void LogMachineUpdate(MachineModel machine, DateTime startTime)
        {
            string totalTime = GetExecutionTimeString(startTime);
            WriteLineColored($"Machine {machine.Id} has been updated in {totalTime}.", _updateColor);
        }

        // Misc logs
        public static void LogTransactionStats(SqlConnection connection)
        {
            if (!_logTransactionNotice) return;
            WriteLineColored("Transaction successful.", _successColor);

            if (!_logTransactionStats) return;
            var stats = connection.RetrieveStatistics();
            Console.WriteLine("Transaction stats:");
            foreach (DictionaryEntry stat in stats)
            {
                Console.WriteLine($@"  {stat.Key} : {stat.Value}");
            }
        }
        private static void LogRequestIpOrigin(ConnectionInfo connectionInfo)
        {
            var ipv4 = connectionInfo.RemoteIpAddress?.MapToIPv4().ToString();
            var ipv6 = connectionInfo.RemoteIpAddress?.MapToIPv6().ToString();
            Console.WriteLine($"Origin: {ipv4} | {ipv6}");
        }
        public static void LogTokenContent(JwtSecurityToken securityToken)
        {
            if (!_logTokenContent) return;
            Console.WriteLine($"Token content:");
            Console.WriteLine(securityToken);
        }
        public static void LogHashSalt(string hash, byte[] salt)
        {
            if (!_logHashSalt) return;
            Console.WriteLine($"Provided pass hash: {hash}");
            Console.WriteLine($"Provided salt: {Convert.ToHexString(salt)}");
        }
    }
}
