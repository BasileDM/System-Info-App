using System.Text.Json.Serialization;

namespace SystemInfoAPI.Models {
    
    public record class DriveModel(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("driveType")] string DriveType,
        [property: JsonPropertyName("driveFormat")] string DriveFormat,
        [property: JsonPropertyName("totalSize")] long TotalSize,
        [property: JsonPropertyName("availableFreeSpace")] long AvailableFreeSpace,
        [property: JsonPropertyName("totalFreeSpace")] long TotalFreeSpace,
        [property: JsonPropertyName("freeSpacePercentage")] int FreeSpacePercentage
        ) {

        public string? SpacePercentageStr = $"{FreeSpacePercentage}%";
    }
}
