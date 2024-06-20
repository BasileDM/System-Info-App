using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemInfoClient.Models
{
    public class SettingsModel
    {
        public string? MachineId { get; set; }
        public string? CustomerId { get; set; }
        public string? ApiUrl { get; set; }
        public Dictionary<string, ApplicationSettings>? ApplicationsList { get; set; }

        [JsonIgnore]
        public int ParsedCustomerId { get; set; }
        [JsonIgnore]
        public int ParsedMachineId { get; set; }
        [JsonIgnore]
        private JsonSerializerOptions SerializerOptions { get; set; } = new() { WriteIndented = true };

        private SettingsModel(){} // Private constructor to avoid direct instantiation without factory method

        [JsonConstructor]
        public SettingsModel(string? machineId, string? customerId, string? apiUrl, Dictionary<string, ApplicationSettings>? applicationsList)
        {
            MachineId = machineId;
            CustomerId = customerId;
            ApiUrl = apiUrl;
            ApplicationsList = applicationsList;

            ParsedCustomerId = GetParsedId(CustomerId, "customer");
            ParsedMachineId = GetParsedId(MachineId, "machine");
        }

        /// <summary>Factory method that returns an instance of <see cref="SettingsModel"/>.</summary>
        /// <remarks>The instance will have its properties populated by deserializing the settings.json file</remarks>
        /// <returns>
        ///     The <see cref="SettingsModel"/> created by <see cref="JsonSerializer"/>.
        /// </returns>
        public static SettingsModel GetInstance()
        {
            try
            {
                string jsonSettings;

                using (StreamReader reader = new(GetFilePath()))
                {
                    jsonSettings = reader.ReadToEnd();
                }

                SettingsModel settings = JsonSerializer.Deserialize<SettingsModel>(jsonSettings) ??
                    throw new InvalidDataException("Deserialization of settings.json failed: null settings.");

                return settings;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"File not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Could not deserialize JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while trying to read settings file: {ex.Message}");
            }
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
        public void RewriteFileWithId(string newMachineId)
        {
            try
            {
                string path = GetFilePath();
                string json = File.ReadAllText(path);
                SettingsModel settings = GetInstance();
                settings.MachineId = newMachineId;

                string newJson = JsonSerializer.Serialize(settings, SerializerOptions); // add the write indented and refactor to avoid having to create settings again when we have in in main, pass the argument from the main method maybe ?

                File.WriteAllText(path, newJson);
                Console.WriteLine($"New machine id: {newMachineId}, path {path} newjson : \r\n{newJson}");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"{ex.Message} + {ex}");
            }
        }
        private static string GetFilePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "settings.json";
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
