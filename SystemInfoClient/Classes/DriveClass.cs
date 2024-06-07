using System.Runtime.Versioning;

namespace SystemInfoClient.Classes
{
    [SupportedOSPlatform("windows")]
    public class DriveClass
    {
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

        public DriveClass(DriveInfo drive, bool isSystemDrive) 
        {
            try
            {
                Name = drive.Name;
                RootDirectory = drive.RootDirectory.ToString();
                Label = drive.VolumeLabel;
                Type = drive.DriveType.ToString();
                Format = drive.DriveFormat;
                Size = drive.TotalSize;
                FreeSpace = drive.AvailableFreeSpace;
                TotalSpace = drive.TotalFreeSpace;
                FreeSpacePercentage = (int)((double)drive.AvailableFreeSpace / drive.TotalSize * 100);
                IsSystemDrive = isSystemDrive;
                Os = IsSystemDrive ? new OsClass() : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error instantiating the machine's drives.", ex);
            }
        }

        public void LogInfo() {
            Console.WriteLine($"Drive Name: {Name}");
            Console.WriteLine($"Drive Label: {Label}");
            Console.WriteLine($"Drive Type: {Type}");
            Console.WriteLine($"Drive Format: {Format}");
            Console.WriteLine($"Total Size: {Size:#,0}");
            Console.WriteLine($"Available Free Space: {FreeSpace:#,0}");
            Console.WriteLine($"Total Free Space: {TotalSpace:#,0}");
            Console.WriteLine($"Free Space Percentage: {FreeSpacePercentage}%");
            Console.WriteLine($"Is system drive: {IsSystemDrive}");

            if (IsSystemDrive) {
                Os?.LogInfo();
            }
            Console.WriteLine();
        }
    }
}
