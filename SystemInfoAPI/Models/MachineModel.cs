using System.Text.Json.Serialization;

namespace SystemInfoApi.Models {
    public record class MachineModel(
        [property: JsonPropertyName("name")] string Name);
}
