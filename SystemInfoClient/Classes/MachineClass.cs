using System.Runtime.Versioning;

namespace SystemInfoClient.Classes
{
    [SupportedOSPlatform("windows")]
    public class MachineClass
    {
        public string Name { get; set; }

        public int CustomerId { get; set; }

        public List<DriveClass> Drives { get; set; }

        public MachineClass() {
            try
            {
                Name = Environment.MachineName;
                Drives = [];

                string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        bool isSystemDriveBool = drive.Name == systemDrive;
                        Drives.Add(new DriveClass(drive, isSystemDriveBool));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error instantiating the machine.", ex);
            }
        }

        public void LogInfo() {
            Console.WriteLine($"Device name: {Name}");
            Console.WriteLine($"Customer ID: {CustomerId}");
            Console.WriteLine();
            foreach (var drive in Drives) {
                drive.LogInfo();
            }
        }
    }
}