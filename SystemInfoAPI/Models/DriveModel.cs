using System.ComponentModel.DataAnnotations.Schema;

namespace SystemInfoApi.Models {

    [Table("Client_Machine_Disque")]
    public class DriveModel {

        [Column("id_client_machine_disque")]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? Label { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }

        [Column("Free_Space")]
        public long FreeSpace { get; set; }

        [Column("Total_Space")]
        public long TotalSpace { get; set; }

        [Column("Free_Space_Percentage")]
        public int FreeSpacePercentage { get; set; }

        [Column("Is_System_Drive")]
        public int IsSystemDrive { get; set; }

        [Column("id_client_machine")]
        public int MachineId { get; set; }  // Foreign key property

        public OsModel? Os { get; set; }
    }
}
