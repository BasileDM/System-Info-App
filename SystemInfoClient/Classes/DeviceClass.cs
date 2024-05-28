using System.Runtime.Versioning;
using SystemInfoClient.Services;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    internal class DeviceClass {
        public string? DeviceName { get; set; }
        public List<DriveClass> Drives { get; set; } = [];

        public DeviceClass() {
            DeviceName = Environment.MachineName;
            string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

            foreach (var drive in DriveInfo.GetDrives()) {
                if (drive.IsReady) {
                    bool isSystemDrive = drive.Name == systemDrive;
                    Drives.Add(new DriveClass(drive, isSystemDrive));
                }
            }
        }

        public void LogInfo() {
            Console.WriteLine($"Device name: {DeviceName}");
            Console.WriteLine();
            foreach (var drive in Drives) {
                Console.WriteLine($"Drive Name: {drive.Name}");
                Console.WriteLine($"Drive Label: {drive.Label}");
                Console.WriteLine($"Drive Type: {drive.DriveType}");
                Console.WriteLine($"Drive Format: {drive.DriveFormat}");
                Console.WriteLine($"Total Size: {drive.TotalSize}");
                Console.WriteLine($"Available Free Space: {drive.AvailableFreeSpace}");
                Console.WriteLine($"Total Free Space: {drive.TotalFreeSpace}");
                Console.WriteLine($"Free Space Percentage: {drive.FreeSpacePercentage}%");
                Console.WriteLine($"Is system drive: {drive.IsSystemDrive}");

                if (drive.IsSystemDrive) {
                    Console.WriteLine($"  OS System Directory: {drive.Os?.SystemDirectory}");
                    Console.WriteLine($"  OS Architecture: {drive.Os?.OsArchitecture}");
                    Console.WriteLine($"  OS Version: {drive.Os?.OsVersion}");
                    Console.WriteLine($"  Product Name: {drive.Os?.ProductName}");
                    Console.WriteLine($"  Release ID: {drive.Os?.ReleaseId}");
                    Console.WriteLine($"  Current Build: {drive.Os?.CurrentBuild}");
                    Console.WriteLine($"  UBR: {drive.Os?.Ubr}");
                }
                Console.WriteLine();
            }
        }
    }
}