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
    }
}
