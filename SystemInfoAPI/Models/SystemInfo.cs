namespace SystemInfoAPI.Models {
    public class SystemInfo {

        public string? MachineName { get; set; }

        public string? OsDrive {  get; set; }

        public string? SystemDirectory { get; set; }

        public string? OsArchitecture { get; set; }

        public string? OsVersion { get; set; }

        public string? ProductName { get; set; }

        public string? ReleaseId { get; set; }

        public string? CurrentBuild { get; set; }

        public string? Ubr { get; set; }

        public List<DriveInfoModel>? AllDrives { get; set; }
    }
}
