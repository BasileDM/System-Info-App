namespace SystemInfoApi.Models
{
    public class MachineModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }  // Foreign key property
        public ICollection<DriveModel> Drives { get; set; }
    }
}
