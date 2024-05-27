﻿using SystemInfoAPI.Models;

namespace SystemInfoAPI.Services {
    public class DriveService {

        /// <summary>Gets the drive on which the OS is present.</summary>
        /// <returns>
        ///   A <see cref="DriveInfoModel"/> type of drive. Or null if the OS is not found.
        /// </returns>
        public static DriveInfoModel? GetOsDrive() {
            List<DriveInfoModel> drivesList = GetDrives();
            foreach (DriveInfoModel drive in drivesList) {
                if (Path.GetPathRoot(Environment.SystemDirectory) == drive.Name) {
                    return drive;
                }
            }
            return null;
        }

        /// <summary>Gets a list of the drives.</summary>
        /// <returns>
        ///   A <see cref="List{DriveInfoModel}"/> of all the drives.
        /// </returns>
        public static List<DriveInfoModel> GetDrives() {
            return DriveInfo.GetDrives().Select(drive => new DriveInfoModel
            {
                Name = drive.Name,
                Label = string.IsNullOrEmpty(drive.VolumeLabel) ? "n.c." : drive.VolumeLabel,
                DriveType = drive.DriveType.ToString(),
                DriveFormat = drive.IsReady ? drive.DriveFormat : "Unknown",
                AvailableFreeSpace = drive.IsReady ? drive.AvailableFreeSpace : 0,
                TotalFreeSpace = drive.IsReady ? drive.TotalFreeSpace : 0,
                TotalSize = drive.IsReady ? drive.TotalSize : 0,
                FreeSpacePercentage = (drive.AvailableFreeSpace * 100 / drive.TotalSize).ToString() + "%"
            }).ToList();
        }

        /// <summary>Gets the drive by letter.</summary>
        /// <param name="letter">The letter.</param>
        /// <returns>
        /// A <see cref="DriveInfoModel"/> object representing the drive with the specified letter. 
        /// If no drive with the given letter is found, returns null.
        /// </returns>
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
                TotalSize = drive.IsReady ? drive.TotalSize : 0,
                FreeSpacePercentage = (drive.AvailableFreeSpace * 100 / drive.TotalSize).ToString() + "%"
            };
        }
    }
}
