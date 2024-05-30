using Microsoft.AspNetCore.Mvc;
using SystemInfoApi.Models;
using System.Runtime.Versioning;
using SystemInfoApi.Data;

namespace SystemInfoApi.Controllers {

    [ApiController]
    [Route("[controller]")]
    [SupportedOSPlatform("windows")]
    public class MachinesController : ControllerBase {

        private readonly SystemInfoContext _context;

        public MachinesController(SystemInfoContext context) {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<ActionResult<MachineModel>> CreateMachine(MachineModel machineModel) {

            if (machineModel == null) {
                return BadRequest("Machine object is null.");
            }

            try {
                _context.Client_Machine.Add(machineModel);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return UnprocessableEntity(ex.Message);
            }

            return CreatedAtAction(nameof(CreateMachine), new { id = machineModel }, machineModel);
        }
        
        //[HttpGet("all")]
        //public ActionResult<MachineModel> GetAll() {
        //    var systemInfo = new SystemModel
        //    {
        //        OsDrive = Path.GetPathRoot(Environment.SystemDirectory),
        //        SystemDirectory = Environment.SystemDirectory,
        //        MachineName = Environment.MachineName,
        //        OsVersion = Environment.OSVersion.ToString(),
        //        OsArchitecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits",
        //        ProductName = RegistryService.GetRegistryValue(
        //            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
        //            "ProductName",
        //            "Unknown Product"),
        //        ReleaseId = RegistryService.GetRegistryValue(
        //            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
        //            "ReleaseId",
        //            "Unknown Release"),
        //        CurrentBuild = RegistryService.GetRegistryValue(
        //            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
        //            "CurrentBuild",
        //            "Unknown Build"),
        //        Ubr = RegistryService.GetRegistryValue(
        //            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
        //            "UBR",
        //            "Unknown UBR"),
        //        AllDrives = DriveService.GetDrives()
        //    };
        //    return Ok(systemInfo);
        //}

        //[HttpGet("drives")]
        //public ActionResult<List<DriveModel>> GetAllDrives() { return Ok(DriveService.GetDrives()); }

        //[HttpGet("osdrive")]
        //public ActionResult<DriveModel> GetOsDriveInfo() { return Ok(DriveService.GetOsDrive()); }

        //[HttpGet("drive/{driveLetter}")]
        //public ActionResult<DriveModel> GetDriveById(string driveLetter) {
        //    DriveModel? drive = DriveService.GetDriveByLetter(driveLetter);
        //    if (drive == null) {
        //        return UnprocessableEntity("Drive not found");
        //    } else { return Ok(drive); }
        //}
    }
}
