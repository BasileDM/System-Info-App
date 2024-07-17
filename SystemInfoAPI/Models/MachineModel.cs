namespace SystemInfoApi.Models
{
    public class MachineModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public int CustomerId { get; set; }  // Foreign key
        public ICollection<DriveModel> Drives { get; set; } = [];
    }
}
