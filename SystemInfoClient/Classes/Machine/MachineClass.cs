using System.Runtime.Versioning;
using System.Text.Json;
using SystemInfoClient.Utilities;

namespace SystemInfoClient.Classes.System
{
    [SupportedOSPlatform("windows")]
    public class MachineClass
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public List<DriveClass> Drives { get; set; }
        private JsonSerializerOptions SerializerOptions { get; set; }

        public MachineClass(Settings settings)
        {
            try
            {
                Id = settings.ParsedMachineId;
                CustomerId = settings.ParsedCustomerId;
                Name = Environment.MachineName;
                Drives = [];
                SerializerOptions = new() { WriteIndented = true };

                string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (!drive.IsReady)
                    {
                        ConsoleUtils.WriteLColored($"Drive {drive.Name} was not ready. Skipping drive.", ConsoleUtils._warningColor);
                        break;
                    }

                    bool isSystemDriveBool = drive.RootDirectory.ToString() == systemDrive;
                    Drives.Add
                    (
                        new DriveClass(drive, isSystemDriveBool, settings)
                    );
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error instantiating the machine." + ex , ex);
            }
        }

        public void LogInfo()
        {
            Console.WriteLine($"Machine ID : {Id}");
            Console.WriteLine($"Customer ID: {CustomerId}");
            Console.WriteLine($"Device name: {Name}");
            Console.WriteLine("Drives: ");
            foreach (var drive in Drives)
            {
                drive.LogInfo();
            }
        }
        public string JsonSerialize()
        {
            return JsonSerializer.Serialize(this, SerializerOptions);
        }
        public void LogJson()
        {
            Console.WriteLine(JsonSerialize());
        }
    }
}