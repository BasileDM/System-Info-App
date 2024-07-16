namespace SystemInfoApi.Models
{
    public class DriveModel
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string RootDirectory { get; set; }
        public string? Label { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }
        public long FreeSpace { get; set; }
        public long TotalSpace { get; set; }
        public int FreeSpacePercentage { get; set; }
        public bool IsSystemDrive { get; set; }
        public DateTime CreationDate { get; set; }
        public int MachineId { get; set; }  // Foreign key
        public OsModel? Os { get; set; }
        public List<ApplicationModel>? AppList { get; set; }
    }
}
