using System.ComponentModel.DataAnnotations.Schema;

namespace SystemInfoApi.Models {

    [Table("Client_Machine")]
    public class MachineModel {

        [Column("id_client_machine")]
        public int? Id { get; set; }
        public string Name { get; set; }

        [Column("id_client")]
        public int CustomerId { get; set; }  // Foreign key property
        public ICollection<DriveModel> Drives { get; set; }
    }
}
