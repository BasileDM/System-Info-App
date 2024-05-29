using System.Text.Json.Serialization;

namespace SystemInfoApi.Models {
    public record class OsModel(
        [property: JsonPropertyName("directory")] string Directory,
        [property: JsonPropertyName("architecture")] string Architecture,
        [property: JsonPropertyName("version")] string Version,
        [property: JsonPropertyName("productName")] string? ProductName,
        [property: JsonPropertyName("releaseId")] string? ReleaseId,
        [property: JsonPropertyName("currentBuild")] string? CurrentBuild,
        [property: JsonPropertyName("ubr")] string? Ubr);
}
