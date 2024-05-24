namespace SystemInfoAPI.Models {
    public class SystemInfo {

        // Machine and OS info
        public string? MachineName { get; set; }
        public string? OsDrive {  get; set; }
        public string? SystemDirectory { get; set; }
        public string? OsArchitecture { get; set; }
        public string? OsVersion { get; set; }

        // Friendly OS info
        public string? ProductName { get; set; }
        public string? ReleaseId { get; set; }
        public string? CurrentBuild { get; set; }
        public string? Ubr { get; set; }

        // Drives info
        public List<DriveInfoModel>? AllDrives { get; set; }
    }
}
