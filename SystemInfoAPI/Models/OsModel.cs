using System.ComponentModel.DataAnnotations.Schema;

namespace SystemInfoApi.Models
{
    [Table("Client_Machine_Disque_Os")]
    public class OsModel
    {
        [Column("id_client_machine_disque_os")]
        public int? Id { get; set; }
        public string? Directory { get; set; }

        public string? Architecture { get; set; }

        public string? Version { get; set; }

        [Column("Product_Name")]
        public string? ProductName { get; set; }

        [Column("Release_Id")]
        public string? ReleaseId { get; set; }

        [Column("Current_Build")]
        public string? CurrentBuild { get; set; }

        public string? Ubr { get; set; }

        [Column("id_client_machine_disque")]
        public int DriveId { get; set; }  // Foreign key property
    }
}
