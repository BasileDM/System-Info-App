using System.Runtime.Versioning;
using SystemInfoClient.Models;

namespace SystemInfoClient.Classes
{
    [SupportedOSPlatform("windows")]
    public class MachineClass
    {
        public string Name { get; set; }

        public int CustomerId { get; set; }

        public List<DriveClass> Drives { get; set; }

        public MachineClass(Dictionary<string, ApplicationSettings> appList)
        {
            try
            {
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
                                Console.WriteLine($"{app.Key} : belongs to drive {drive.Name}");
                                AppClass appClass = new(app);
                                driveAppsList.Add(appClass);
                            }

                            bool isSystemDriveBool = drive.Name == systemDrive;
                            Drives.Add(new DriveClass(drive, isSystemDriveBool));
                        }
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
            Console.WriteLine($"Device name: {Name}");
            Console.WriteLine($"Customer ID: {CustomerId}");
            Console.WriteLine();
            foreach (var drive in Drives)
            {
                drive.LogInfo();
            }
        }
    }
}