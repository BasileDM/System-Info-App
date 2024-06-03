using System.ComponentModel.DataAnnotations.Schema;

namespace SystemInfoApi.Models {

    [Table("Client")]
    public class CustomerModel {

        [Column("id_client")]
        public int? Id { get; set; }

        public string Name { get; set; }

        public ICollection<MachineModel> Machines { get; set; }
    }
}
