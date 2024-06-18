namespace SystemInfoApi.Models
{
    public class ApplicationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Comments { get; set; }
        public string? CompanyName { get; set; }
        public int FileBuildPart { get; set; }
        public string? FileDescription { get; set; }
        public int FileMajorPart { get; set; }
        public int FileMinorPart { get; set; }
        public string? FileName { get; set; }
        public int FilePrivatePart { get; set; }
        public string? FileVersion { get; set; }
        public string? InternalName { get; set; }
        public bool IsDebug { get; set; }
        public bool IsPatched { get; set; }
        public bool IsPrivateBuild { get; set; }
        public bool IsPreRelease { get; set; }
        public bool IsSpecialBuild { get; set; }
        public string? Language { get; set; }
        public string? LegalCopyright { get; set; }
        public string? LegalTrademarks { get; set; }
        public string? OriginalFilename { get; set; }
        public string? PrivateBuild { get; set; }
        public int ProductBuildPart { get; set; }
        public int ProductMajorPart { get; set; }
        public int ProductMinorPart { get; set; }
        public string? ProductName { get; set; }
        public int ProductPrivatePart { get; set; }
        public string? ProductVersion { get; set; }
        public string? SpecialBuild { get; set; }
        public int DriveId { get; set; } // Foreign key
    }
}
