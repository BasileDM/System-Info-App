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
        // true: switches all logs to true
        // false: all logs to false
        // null: logs will keep the value provided in SetProperty(value).
        private readonly static bool? _logsMasterSwitch = null;
        public readonly static bool _logTransactionNotice = SetProperty(true);
        private readonly static bool _logTransactionStats = SetProperty(true);
        private readonly static bool _logAuthRequestContent = SetProperty(false);
        private readonly static bool _logCreationDetails = SetProperty(true);

        private readonly static bool _logTokenContent = SetProperty(true);
        private readonly static bool _logEncodedToken = SetProperty(false);
        private readonly static bool _logHashSalt = SetProperty(false);

        public readonly static ConsoleColor _requestColor = ConsoleColor.Yellow;
        public readonly static ConsoleColor _creationColor = ConsoleColor.Green;
        public readonly static ConsoleColor _updateColor = ConsoleColor.DarkYellow;
        public readonly static ConsoleColor _deletionColor = ConsoleColor.DarkRed;

        public readonly static ConsoleColor _successColor = ConsoleColor.DarkGreen;
        public readonly static ConsoleColor _errorColor = ConsoleColor.Red;
        public readonly static ConsoleColor _timeStampColor = ConsoleColor.DarkGray;

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
        public static void WriteColored(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        private static int GetExecutionTimeInMs(DateTime startTime)
        {
            var elapsed = (DateTime.Now.ToLocalTime() - startTime).TotalMilliseconds;
            return (int)elapsed;
        }

        // LOGS
        // Recieving requests logs
        public static void LogMachineCreationRequest(ConnectionInfo connectionInfo, DateTime startTime)
        {
            Console.WriteLine();
            WriteColored("Issuing machine creation request...", _requestColor);
            LogTimeStamp(startTime);
            LogRequestIpOrigin(connectionInfo);
        }
        public static void LogUpdateRequest(int machineId, ConnectionInfo connectionInfo, DateTime startTime)
        {
            Console.WriteLine();
            WriteColored($"Issuing update request for machine {machineId}...", _requestColor);
            LogTimeStamp(startTime);
            LogRequestIpOrigin(connectionInfo);
        }
        public static void LogAuthRequest(AuthRequest request, ConnectionInfo connectionInfo, DateTime startTime)
        {
            Console.WriteLine();
            WriteColored($"Issuing token request...", _requestColor);
            LogTimeStamp(startTime);
            LogRequestIpOrigin(connectionInfo);

            if (!_logAuthRequestContent) return;
            Console.WriteLine($"Request Content:");
            Console.WriteLine($"Full hash: {request.Pass}");
        }
        public static void LogGetMachineByIdRequest(int machineId, ConnectionInfo connectionInfo, DateTime startTime)
        {
            Console.WriteLine();
            WriteColored($"Issuing get request for machine {machineId}...", _requestColor);
            LogTimeStamp(startTime);
            LogRequestIpOrigin(connectionInfo);
        }
        public static void LogTimeStamp(DateTime startTime)
        {
            string timeStamp = startTime.ToString();
            WriteColored($@" ({timeStamp})", _timeStampColor);
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
            WriteColored($"Machine {newMachine.Id} has been created.", _creationColor);
            WriteLineColored($@" ({GetExecutionTimeInMs(startTime)}ms)", _timeStampColor);

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
        public static void LogAppDeletion(string appName, int appId, int appDriveId)
        {
            WriteLineColored($"Deleting app relation '{appName}' (id {appId}) on drive {appDriveId}.", _deletionColor);
        }
        public static void LogDriveCreation(string DriveName, int driveId, string serial)
        {
            WriteLineColored($"Creating new drive '{DriveName}' (id: {driveId} | Serial: {serial}).", _creationColor);
        }
        public static void LogDriveDeletion(string DriveName, int driveId, string serial)
        {
            WriteLineColored($"Deleting drive '{DriveName}' (id: {driveId} | Serial: {serial}).", _deletionColor);
        }

        // Update logs
        public static void LogMachineUpdate(MachineModel machine, DateTime startTime)
        {
            WriteColored($"Machine {machine.Id} has been updated.", _updateColor);
            WriteLineColored($@" ({GetExecutionTimeInMs(startTime)}ms)", _timeStampColor);
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
            Console.WriteLine();
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
        public static void LogPassProcessTime(Stopwatch stopwatch)
        {
            string elapsed = stopwatch.ElapsedMilliseconds.ToString();
            WriteLineColored($@" ({elapsed}ms)", _timeStampColor);
            stopwatch.Stop();
            stopwatch.Reset();
        }
    }
}
