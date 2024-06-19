namespace SystemInfoClient.Models
{
    public class SettingsModel
    {
        public string? MachineId { get; set; }
        public string? CustomerId { get; set; }
        public string? ApiUrl { get; set; }
        public Dictionary<string, ApplicationSettings>? ApplicationsList { get; set; }
        public int ParsedCustomerId { get; set; }
        public int ParsedMachineId { get; set; }

        public SettingsModel(string? machineId, string customerId, string? apiUrl, Dictionary<string, ApplicationSettings>? applicationsList)
        {
            MachineId = machineId;
            CustomerId = customerId;
            ApiUrl = apiUrl;
            ApplicationsList = applicationsList;

            ParsedCustomerId = GetParsedId(CustomerId, "customer");
            ParsedMachineId = GetParsedId(MachineId, "machine");
        }

        private static int GetParsedId(string? id, string idOwnerName)
        {
            if (Int32.TryParse(id, out int parsedId) && parsedId > 0)
            {
                return parsedId;
            }
            else if (parsedId == 0 && idOwnerName == "machine")
            {
                return parsedId;
            }
            else
            {
                throw new InvalidDataException(
                    $"Invalid {idOwnerName} ID, please provide a valid one in the settings.json file");
            }
        }
    }

    public class ApplicationSettings
    {
        public string? Id { get; set; }
        public string? Path { get; set; }
        public int ParsedId { get; set; }

        public ApplicationSettings(string? id, string? path)
        {
            Id = id;
            Path = path;
        }
    }
}
