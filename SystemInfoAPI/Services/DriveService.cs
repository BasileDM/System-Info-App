using SystemInfoAPI.Models;

namespace SystemInfoAPI.Services {
    public class DriveService {
        public static DriveInfoModel? GetOsDrive() {
            List<DriveInfoModel> drivesList = GetDrives();
            foreach (DriveInfoModel drive in drivesList) {
                if (Path.GetPathRoot(Environment.SystemDirectory) == drive.Name) {
                    return drive;
                }
            }
            return null;
        }
        public static List<DriveInfoModel> GetDrives() {
            return DriveInfo.GetDrives().Select(drive => new DriveInfoModel
            {
                Name = drive.Name,
                Label = string.IsNullOrEmpty(drive.VolumeLabel) ? "n.c." : drive.VolumeLabel,
                DriveType = drive.DriveType.ToString(),
                DriveFormat = drive.IsReady ? drive.DriveFormat : "Unknown",
                AvailableFreeSpace = drive.IsReady ? drive.AvailableFreeSpace : 0,
                TotalFreeSpace = drive.IsReady ? drive.TotalFreeSpace : 0,
                TotalSize = drive.IsReady ? drive.TotalSize : 0
            }).ToList();
        }

        public static DriveInfoModel? GetDriveByLetter(string letter) {
            string fullName = letter.ToUpper() + ":\\";
            var drive = DriveInfo.GetDrives().FirstOrDefault(
                drive => drive.Name.Equals(fullName, StringComparison.OrdinalIgnoreCase)
            );

            if (drive == null) {
                return null;
            }

            return new DriveInfoModel
            {
                Name = drive.Name,
                Label = string.IsNullOrEmpty(drive.VolumeLabel) ? "n.c." : drive.VolumeLabel,
                DriveType = drive.DriveType.ToString(),
                DriveFormat = drive.IsReady ? drive.DriveFormat : "Unknown",
                AvailableFreeSpace = drive.IsReady ? drive.AvailableFreeSpace : 0,
                TotalFreeSpace = drive.IsReady ? drive.TotalFreeSpace : 0,
                TotalSize = drive.IsReady ? drive.TotalSize : 0
            };

        }

    }
}
