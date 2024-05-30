using System.Text.Json.Serialization;

namespace SystemInfoApi.Models {
    public record class MachineModel {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<DriveModel> Drives { get; set; }
    }
}
