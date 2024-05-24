using SystemInfoAPI.Models;

namespace SystemInfoAPI.Services {
    public class DriveService {
        public static DriveInfoModel? GetOsDrive() {
            List<DriveInfoModel> drivesList = GetDrivesInfo();
            foreach (DriveInfoModel drive in drivesList) {
                if (Path.GetPathRoot(Environment.SystemDirectory) == drive.Name) {
                    return drive;
                }
            }
            return null;
        }
        public static List<DriveInfoModel> GetDrivesInfo() {
            return DriveInfo.GetDrives().Select(disk => new DriveInfoModel
            {
                Name = disk.Name,
                Label = string.IsNullOrEmpty(disk.VolumeLabel) ? "n.c." : disk.VolumeLabel,
                DriveType = disk.DriveType.ToString(),
                DriveFormat = disk.IsReady ? disk.DriveFormat : "Unknown",
                AvailableFreeSpace = disk.IsReady ? disk.AvailableFreeSpace : 0,
                TotalFreeSpace = disk.IsReady ? disk.TotalFreeSpace : 0,
                TotalSize = disk.IsReady ? disk.TotalSize : 0
            }).ToList();
        }
    }
}
