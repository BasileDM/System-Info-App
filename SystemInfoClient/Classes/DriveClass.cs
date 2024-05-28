using System.Runtime.Versioning;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    public class DriveClass(DriveInfo drive, bool isSystemDrive) {
        public string? Name { get; set; } = drive.Name;
        public string? Label { get; set; } = drive.VolumeLabel;
        public string? DriveType { get; set; } = drive.DriveType.ToString();
        public string? DriveFormat { get; set; } = drive.DriveFormat;
        public long TotalSize { get; set; } = drive.TotalSize;
        public long AvailableFreeSpace { get; set; } = drive.AvailableFreeSpace;
        public long TotalFreeSpace { get; set; } = drive.TotalFreeSpace;
        public int FreeSpacePercentage { get; set; } 
            = (int)((double)drive.AvailableFreeSpace / drive.TotalSize * 100);
        public bool IsSystemDrive { get; set; } = isSystemDrive;
        public OsClass? Os { get; set; } = isSystemDrive ? new OsClass() : null;
    }
}
