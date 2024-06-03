using System.Runtime.Versioning;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    public class DriveClass(DriveInfo drive, bool isSystemDrive) {
        public string Name { get; set; } = drive.Name;
        public string? Label { get; set; } = drive.VolumeLabel;
        public string Type { get; set; } = drive.DriveType.ToString();
        public string Format { get; set; } = drive.DriveFormat;
        public long Size { get; set; } = drive.TotalSize;
        public long FreeSpace { get; set; } = drive.AvailableFreeSpace;
        public long TotalSpace { get; set; } = drive.TotalFreeSpace;
        public int FreeSpacePercentage { get; set; }
            = (int)((double)drive.AvailableFreeSpace / drive.TotalSize * 100);
        public bool IsSystemDrive { get; set; } = isSystemDrive;
        public OsClass? Os { get; set; } = isSystemDrive ? new OsClass() : null;

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
