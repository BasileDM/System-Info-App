namespace SystemInfoClient.Models
{
    internal class SettingsModel
    {
        public string? CustomerId { get; set; }

        public string? ApiUrl { get; set; }

        public Dictionary<string, string>? Applications { get; set; }
    }
}
