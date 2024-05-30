using System.Text.Json.Serialization;

namespace SystemInfoApi.Models {

    public record class DriveModel { 
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? Label { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }
        public long FreeSpace { get; set; }
        public long TotalSpace { get; set; }
        public int FreeSpacePercentage { get; set; }
        public int IsSystemDrive { get; set; }
        public OsModel? Os { get; set; }
    }
}
