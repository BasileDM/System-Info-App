using System.Runtime.Versioning;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    internal class MachineClass {
        public string Name { get; set; }

        public int CustomerId { get; set; }

        public List<DriveClass> Drives { get; set; } = [];

        public MachineClass() {
            //Name = Environment.MachineName;
            Name = "030624-1100Machine";

            while (true) {
                Console.Write("Customer ID: ");
                string? CustomerIdStr = Console.ReadLine();
                if (CustomerIdStr != String.Empty && int.TryParse(CustomerIdStr, out int CustomerIdInt)) {
                    CustomerId = CustomerIdInt;
                    break;
                }
                Console.WriteLine("Invalid customer ID number.");
            }

            string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

            foreach (var drive in DriveInfo.GetDrives()) {
                if (drive.IsReady) {
                    bool isSystemDriveBool = drive.Name == systemDrive;
                    int isSystemDrive = isSystemDriveBool ? 1 : 0;
                    Drives.Add(new DriveClass(drive, isSystemDriveBool));
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