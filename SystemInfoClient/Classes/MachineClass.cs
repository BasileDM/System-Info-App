using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        public string GetJson() {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            return json;
        }
    }
}