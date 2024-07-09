namespace SystemInfoApi.Models
{
    public class OsModel
    {
        public int Id { get; set; }
        public string Directory { get; set; }
        public string Architecture { get; set; }
        public string Version { get; set; }
        public string? ProductName { get; set; }
        public string? ReleaseId { get; set; }
        public string? CurrentBuild { get; set; }
        public string? Ubr { get; set; }
        public DateTime CreationDate { get; set; }
        public int DriveId { get; set; }  // Foreign key
    }
}
