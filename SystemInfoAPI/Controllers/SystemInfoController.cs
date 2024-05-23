using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemInfoAPI.Models;

namespace SystemInfoAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SystemInfoController : ControllerBase {

        [HttpGet]
        public ActionResult<SystemInfo> Get() {
            var systemInfo = new SystemInfo()
            {
                OsDrive = Environment.MachineName,
                SystemDirectory = Environment.SystemDirectory,
                OsVersion = Environment.OSVersion.ToString(),
                OsArchitecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits",

            };

            return Ok(systemInfo);
        }
    }
}
