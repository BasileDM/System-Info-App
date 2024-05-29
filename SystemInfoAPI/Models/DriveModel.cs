using System.Text.Json.Serialization;

namespace SystemInfoApi.Models {

    public record class DriveModel(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("label")] string? Label,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("format")] string Format,
        [property: JsonPropertyName("size")] long Size,
        [property: JsonPropertyName("availableFreeSpace")] long FreeSpace,
        [property: JsonPropertyName("totalSpace")] long TotalSpace,
        [property: JsonPropertyName("freeSpacePercentage")] int FreeSpacePercentage,
        [property: JsonPropertyName("isSystemDrive")] int IsSystemDrive
        ) {
        public string SpacePercentageStr = $"{FreeSpacePercentage}%";
    }
}
