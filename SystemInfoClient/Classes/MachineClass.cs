using System.Runtime.Versioning;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    internal class MachineClass {
        public string Name { get; set; }

        public int CustomerId { get; set; }

        public List<DriveClass> Drives { get; set; } = [];

        public MachineClass() {
            Name = Environment.MachineName;

            Console.Write("Customer ID: ");
            string CustomerIdStr = Console.ReadLine();
            CustomerId = int.Parse(CustomerIdStr);

            string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

            foreach (var drive in DriveInfo.GetDrives()) {
                if (drive.IsReady) {
                    bool isSystemDriveBool = drive.Name == systemDrive;
                    int isSystemDrive = isSystemDriveBool ? 1 : 0;
                    Drives.Add(new DriveClass(drive, isSystemDrive));
                }
            }
        }

        public void LogInfo() {
            Console.WriteLine($"Device name: {Name}");
            Console.WriteLine();
            foreach (var drive in Drives) {
                drive.LogInfo();
            }
        }
    }
}