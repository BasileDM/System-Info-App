using System.Text.Json.Serialization;

namespace SystemInfoClient.Models {
    public record class Drive(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("driveType")] string DriveType,
        [property: JsonPropertyName("driveFormat")] string DriveFormat,
        [property: JsonPropertyName("totalSize")] long TotalSize,
        [property: JsonPropertyName("availableFreeSpace")] long AvailableFreeSpace,
        [property: JsonPropertyName("totalFreeSpace")] long TotalFreeSpace,
        [property: JsonPropertyName("freeSpacePercentage")] string? FreeSpacePercentage) {

    }
}
