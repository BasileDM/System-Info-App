using System.Runtime.Versioning;
using SystemInfoClient.Models;

namespace SystemInfoClient.Classes
{
    [SupportedOSPlatform("windows")]
    public class MachineClass
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public List<DriveClass> Drives { get; set; }

        public MachineClass(SettingsModel settings)
        {
            try
            {
                Dictionary<string, ApplicationSettings> appList = settings.ApplicationsList;

                if (Int32.TryParse(settings.MachineId, out int parsedMachineId) && parsedMachineId > 0)
                {
                    Id = parsedMachineId;
                }
                    ;
                Name = Environment.MachineName;
                Drives = [];

                string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

                foreach (var drive in DriveInfo.GetDrives())
                {
                    List<AppClass> driveAppsList = [];

                    if (drive.IsReady)
                    {
                        foreach (var app in appList)
                        {
                            if (app.Value.Path != null && app.Value.Path.Contains(drive.RootDirectory.ToString()))
                            {
                                AppClass appClass = new(app);
                                driveAppsList.Add(appClass);
                            }
                        }
                        bool isSystemDriveBool = drive.Name == systemDrive;
                        Drives.Add(new DriveClass(drive, isSystemDriveBool, driveAppsList));
                    }
                    else
                    {
                        throw new DriveNotFoundException("Error with drive ready state.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error instantiating the machine.", ex);
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
    }
}