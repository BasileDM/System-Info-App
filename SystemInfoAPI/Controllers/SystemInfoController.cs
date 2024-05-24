using Microsoft.AspNetCore.Mvc;
using SystemInfoAPI.Models;
using SystemInfoAPI.Services;
using System.Runtime.Versioning;

namespace SystemInfoAPI.Controllers {

    [SupportedOSPlatform("windows")]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemInfoController : ControllerBase {

        [HttpGet("all")]
        public ActionResult<SystemInfo> GetAll() {
            var systemInfo = new SystemInfo
            {
                OsDrive = Path.GetPathRoot(Environment.SystemDirectory),
                SystemDirectory = Environment.SystemDirectory,
                MachineName = Environment.MachineName,
                OsVersion = Environment.OSVersion.ToString(),
                OsArchitecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits",
                ProductName = GetRegistryValueOrDefault(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "Unknown Product"),
                ReleaseId = GetRegistryValueOrDefault(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "Unknown Release"),
                CurrentBuild = GetRegistryValueOrDefault(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "Unknown Build"),
                Ubr = GetRegistryValueOrDefault(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "Unknown UBR"),
                AllDrives = GetDrivesInfo()
            };

            return Ok(systemInfo);
        }

        [HttpGet("drives")]
        public ActionResult<List<DriveInfoModel>> GetDriveById() { return Ok(GetDrivesInfo()); }

        [HttpGet("osdrive")]
        public ActionResult<DriveInfoModel> GetOsDriveInfo() { return Ok(GetOsDrive()); }

        public static DriveInfoModel? GetOsDrive() {
            List<DriveInfoModel> drivesList = GetDrivesInfo();
            foreach (DriveInfoModel drive in drivesList) {
                if (Path.GetPathRoot(Environment.SystemDirectory) == drive.Name) { 
                    return drive;
                }
            }
            return null;
        }

        private static string GetRegistryValueOrDefault(string path, string key, string defaultValue) {
            var value = RegistryService.GetRegistryValue(path, key);
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        private static List<DriveInfoModel> GetDrivesInfo() {
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
