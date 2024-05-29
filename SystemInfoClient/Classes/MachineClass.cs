using System.Runtime.Versioning;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    internal class MachineClass {
        public string Name { get; set; }
        public List<DriveClass> Drives { get; set; } = [];

        public MachineClass() {
            Name = Environment.MachineName;
            string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

            foreach (var drive in DriveInfo.GetDrives()) {
                if (drive.IsReady) {
                    bool isSystemDrive = drive.Name == systemDrive;
                    Drives.Add(new DriveClass(drive, isSystemDrive));
                }
            }
        }

        public void LogInfo() {
            Console.WriteLine($"Device name: {Name}");
            Console.WriteLine();
            foreach (var drive in Drives) {
                Console.WriteLine($"Drive Name: {drive.Name}");
                Console.WriteLine($"Drive Label: {drive.Label}");
                Console.WriteLine($"Drive Type: {drive.Type}");
                Console.WriteLine($"Drive Format: {drive.Format}");
                Console.WriteLine($"Total Size: {drive.Size:#,0}");
                Console.WriteLine($"Available Free Space: {drive.FreeSpace:#,0}");
                Console.WriteLine($"Total Free Space: {drive.TotalSpace:#,0}");
                Console.WriteLine($"Free Space Percentage: {drive.FreeSpacePercentage}%");
                Console.WriteLine($"Is system drive: {drive.IsSystemDrive}");

                if (drive.IsSystemDrive) {
                    Console.WriteLine($"  OS System Directory: {drive.Os?.Directory}");
                    Console.WriteLine($"  OS Architecture: {drive.Os?.Architecture}");
                    Console.WriteLine($"  OS Version: {drive.Os?.Version}");
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