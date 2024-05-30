using System.Runtime.Versioning;
using System.Text.Json;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    internal class CustomerClass {
        public string Name { get; set; }
        public MachineClass Machine { get; set; }

        public CustomerClass() {
            while (true) {
                Console.Write("Entrer le nom du client : ");
                Name = Console.ReadLine();
                if (Name != null) {
                    break;
                }
                Console.WriteLine("Veuillez entrer une valeur");
            }
            Machine = new MachineClass();
        }

        public void LogInfo() {
            Console.WriteLine($"Client name: {Name}");
            Machine.LogInfo();
        }
        public string GetJson() {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            return json;
        }
    }
}
