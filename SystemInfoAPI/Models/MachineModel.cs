using System.Text.Json.Serialization;

namespace SystemInfoAPI.Models {
    public record class MachineModel(
        [property: JsonPropertyName("name")] string? Name) {
    }
}
