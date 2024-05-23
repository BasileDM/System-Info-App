using Microsoft.AspNetCore.Mvc;
using SystemInfoAPI.Models;
using SystemInfoAPI.Services;
using System.Runtime.Versioning;

namespace SystemInfoAPI.Controllers {
    [SupportedOSPlatform("windows")]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemInfoController : ControllerBase {

        [HttpGet]
        public ActionResult<SystemInfo> Get() {
            var systemInfo = new SystemInfo()
            {
                // Set general OS details
                OsDrive = Path.GetPathRoot(Environment.SystemDirectory),
                SystemDirectory = Environment.SystemDirectory,
                MachineName = Environment.MachineName,
                OsVersion = Environment.OSVersion.ToString(),
                OsArchitecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits",
                ProductName = RegistryService.GetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName"),
                ReleaseId = RegistryService.GetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId"),
                CurrentBuild = RegistryService.GetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild"),
                Ubr = RegistryService.GetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR")
            };

            return Ok(systemInfo);
        }
    }
}
