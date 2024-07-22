using System.Runtime.Versioning;
using System.Management;
using SystemInfoClient.Utilities;

namespace SystemInfoClient.Classes.System
{
    [SupportedOSPlatform("windows")]
    public class DriveClass
    {
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string RootDirectory { get; set; }
        public string? Label { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }
        public long FreeSpace { get; set; }
        public long TotalSpace { get; set; }
        public int FreeSpacePercentage { get; set; }
        public bool IsSystemDrive { get; set; }
        public OsClass? Os { get; set; }
        public List<AppClass> AppList { get; set; }

        public DriveClass(DriveInfo drive, bool isSystemDrive, Settings settings)
        {
            try
            {
                SerialNumber = GetSerialNumber(drive);
                Name = drive.Name;
                RootDirectory = drive.RootDirectory.ToString();
                Label = drive.VolumeLabel;
                Type = drive.DriveType.ToString();
                Format = drive.DriveFormat;
                Size = drive.TotalSize;
                FreeSpace = drive.AvailableFreeSpace;
                TotalSpace = drive.TotalFreeSpace;
                FreeSpacePercentage = CalculateFreeSpacePercentage(drive.AvailableFreeSpace, drive.TotalSize);
                IsSystemDrive = isSystemDrive;
                Os = IsSystemDrive ? new OsClass() : null;
                AppList = GetAppList(settings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error instantiating the machine's drives.", ex);
            }
        }

        public List<AppClass> GetAppList(Settings settings)
        {
            List<AppClass> appList = [];

            if (settings.ApplicationsList != null)
            {
                foreach (var appSettings in settings.ApplicationsList)
                {
                    if (appSettings.Value.Path != null && appSettings.Value.Path.Contains(RootDirectory.ToString()))
                    {
                        try
                        {
                            AppClass appClass = new(appSettings);
                            appList.Add(appClass);
                        }
                        catch (FileNotFoundException ex)
                        {
                            // Just skip the app if the path was not found.
                            ConsoleUtils.WriteLColored(ex.Message, ConsoleUtils._warningColor);
                        }
                    }
                }
            }
            return appList;
        }
        public void LogInfo()
        {
            Console.WriteLine($"  Drive Name: {Name}");
            Console.WriteLine($"  Drive Label: {Label}");
            Console.WriteLine($"  Drive Type: {Type}");
            Console.WriteLine($"  Drive Format: {Format}");
            Console.WriteLine($"  Total Size: {Size:#,0}");
            Console.WriteLine($"  Available Free Space: {FreeSpace:#,0}");
            Console.WriteLine($"  Total Free Space: {TotalSpace:#,0}");
            Console.WriteLine($"  Free Space Percentage: {FreeSpacePercentage}%");
            Console.WriteLine($"  Is system drive: {IsSystemDrive}");

            Console.WriteLine($"  Operating System: ");
            if (IsSystemDrive)
            {
                Os?.LogInfo();
            }

            Console.WriteLine($"  Applications: ");
            foreach (AppClass app in AppList)
            {
                app.LogInfo();
            }
        }
        private int CalculateFreeSpacePercentage(long availableFreeSpace, long totalSize)
        {
            if (totalSize <= 0)
            {
                throw new ArgumentOutOfRangeException($"Drive data error: Total drive size was null or negative on drive {Name}");
            }
            else
            {
                return (int)((double)availableFreeSpace / totalSize * 100);
            }
        }
        private static string GetSerialNumber(DriveInfo drive)
        {
            try
            {
                string driveLetter = drive.Name.TrimEnd('\\');
                string query = $"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID='{driveLetter}'";

                ManagementObjectSearcher searcher = new(query);
                ManagementObject? disk = searcher.Get().Cast<ManagementObject>().FirstOrDefault() 
                    ?? throw new Exception("Drive not found.");
                
                string? serialNumber = disk["VolumeSerialNumber"]?.ToString();

                if (string.IsNullOrEmpty(serialNumber))
                {
                    throw new Exception("Empty serial number.");
                }
                else
                {
                    return serialNumber;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving drive {drive.Name} serial number: " + ex.Message);
            }
        }
    }
}
