namespace SystemInfoClient.Models
{
    public class SettingsModel
    {
        public string? MachineId { get; set; }
        public string? CustomerId { get; set; }
        public string? ApiUrl { get; set; }
        public Dictionary<string, ApplicationSettings>? ApplicationsList { get; set; }

    }

    public class ApplicationSettings
    {
        public string? Id { get; set; }
        public string? Path { get; set; }
    }
}
