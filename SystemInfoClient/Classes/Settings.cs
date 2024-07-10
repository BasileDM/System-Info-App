using System.Text.Json;
using System.Text.Json.Serialization;
using SystemInfoClient.Utilities;

namespace SystemInfoClient.Classes
{
    public class Settings
    {
        public string? MachineId { get; set; }
        public string? CustomerId { get; set; }
        public string ApiUrl { get; set; } = null!;
        public Dictionary<string, ApplicationSettings>? ApplicationsList { get; set; }

        [JsonIgnore]
        public int ParsedCustomerId { get; set; }
        [JsonIgnore]
        public int ParsedMachineId { get; set; }
        [JsonIgnore]
        private JsonSerializerOptions SerializerOptions { get; set; } = new() { WriteIndented = true };

        private Settings() { } // Private constructor to avoid direct instantiation without using factory method

        [JsonConstructor] // Public constructor for the JsonSerializer
        public Settings(string? machineId, string? customerId, string apiUrl, Dictionary<string, ApplicationSettings>? applicationsList)
        {
            MachineId = machineId;
            CustomerId = customerId;
            ApiUrl = apiUrl;
            ApplicationsList = applicationsList;

            ParsedCustomerId = GetParsedId(CustomerId, "customer");
            ParsedMachineId = GetParsedId(MachineId, "machine");
        }

        /// <summary>Factory method that returns an instance of <see cref="Settings"/>.</summary>
        /// <remarks>The instance will have its properties populated by deserializing the settings.json file</remarks>
        /// <returns>
        ///     A <see cref="Settings"/> instance created by <see cref="JsonSerializer"/>.
        /// </returns>
        public static Settings GetInstance()
        {
            try
            {
                string jsonSettings;

                using (StreamReader reader = new(GetFilePath()))
                {
                    jsonSettings = reader.ReadToEnd();
                }

                Settings settings = JsonSerializer.Deserialize<Settings>(jsonSettings) ??
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
            catch (InvalidDataException ex)
            {
                throw new JsonException($"Settings.json file error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while trying to read settings file: {ex.Message}");
            }
        }
        private static int GetParsedId(string? id, string idOwner)
        {
            if (Int32.TryParse(id, out int parsedId) && parsedId > 0)
            {
                return parsedId;
            }
            else if (idOwner == "machine" && (id == string.Empty || id == "0" || id == null))
            {
                return parsedId;
            }
            else
            {
                throw new InvalidDataException(
                    $"Invalid '{idOwner}' ID, please provide a valid one in the settings.json file");
            }
        }
        public void RewriteFileWithId(string? newMachineId)
        {
            if (newMachineId != ParsedMachineId.ToString() && newMachineId != null)
            {
                try
                {
                    MachineId = newMachineId;
                    string path = GetFilePath();
                    string json = GetJson();

                    File.WriteAllText(path, json);
                    Console.WriteLine($"Settings.json file rewritten with: MachineId:{newMachineId}");
                    //Console.WriteLine($"Json content : \r\n{json}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message} + {ex}");
                }
            }
        }
        private static string GetFilePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "settings.json";
        }
        private string GetJson()
        {
            return JsonSerializer.Serialize(this, SerializerOptions);
        }
    }

    public class ApplicationSettings
    {
        public string? Id { get; set; }
        public string? Path { get; set; }

        [JsonIgnore]
        public int ParsedId { get; set; }

        public ApplicationSettings(string? id, string? path)
        {
            Id = id;
            Path = path;

            ParsedId = Int32.TryParse(id, out int parsedId) && parsedId > 0 ? parsedId :
                throw new InvalidDataException($"An application has an invalid ID of: {id} in the settings.json file.");
        }
    }
}
