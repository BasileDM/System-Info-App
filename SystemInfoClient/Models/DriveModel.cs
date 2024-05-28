namespace SystemInfoClient.Models {
    public class DriveModel {
        public string? Name { get; set; }
        public string? Label { get; set; }
        public string? DriveType { get; set; }
        public string? DriveFormat { get; set; }
        public long TotalSize { get; set; }
        public long AvailableFreeSpace { get; set; }
        public long TotalFreeSpace { get; set; }
        public int FreeSpacePercentage { get; set; }
    }
}
