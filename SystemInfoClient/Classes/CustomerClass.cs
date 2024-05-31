using System.Runtime.Versioning;

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
    }
}
