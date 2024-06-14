namespace SystemInfoClient.Models
{
    internal class SettingsModel
    {
        public string? CustomerId { get; set; }
        public string? ApiUrl { get; set; }
        public Dictionary<string, ApplicationSettings>? ApplicationsList { get; set; }
    }

    internal class ApplicationSettings
    {
        public string? Id { get; set; }
        public string? Path { get; set; }
    }
}
