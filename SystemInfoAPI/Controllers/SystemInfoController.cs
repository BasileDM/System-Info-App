using Microsoft.AspNetCore.Mvc;
using SystemInfoAPI.Models;
using SystemInfoAPI.Services;
using System.Runtime.Versioning;

namespace SystemInfoAPI.Controllers {

    [SupportedOSPlatform("windows")]
    [Route("[controller]")]
    [ApiController]
    public class SystemInfoController : ControllerBase {

        [HttpGet("all")]
        public ActionResult<SystemModel> GetAll() {
            var systemInfo = new SystemModel
            {
                OsDrive = Path.GetPathRoot(Environment.SystemDirectory),
                SystemDirectory = Environment.SystemDirectory,
                MachineName = Environment.MachineName,
                OsVersion = Environment.OSVersion.ToString(),
                OsArchitecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits",
                ProductName = RegistryService.GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "ProductName",
                    "Unknown Product"),
                ReleaseId = RegistryService.GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "ReleaseId",
                    "Unknown Release"),
                CurrentBuild = RegistryService.GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "CurrentBuild",
                    "Unknown Build"),
                Ubr = RegistryService.GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "UBR",
                    "Unknown UBR"),
                AllDrives = DriveService.GetDrives()
            };
            return Ok(systemInfo);
        }

        [HttpGet("drives")]
        public ActionResult<List<DriveModel>> GetAllDrives() { return Ok(DriveService.GetDrives()); }

        [HttpGet("osdrive")]
        public ActionResult<DriveModel> GetOsDriveInfo() { return Ok(DriveService.GetOsDrive()); }

        [HttpGet("drive/{driveLetter}")]
        public ActionResult<DriveModel> GetDriveById(string driveLetter) {
            DriveModel? drive = DriveService.GetDriveByLetter(driveLetter);
            if (drive == null) {
                return UnprocessableEntity("Drive not found");
            } else { return Ok(drive); }
        }
    }
}
