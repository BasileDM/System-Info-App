using System.Runtime.Versioning;

namespace SystemInfoClient.Classes {

    [SupportedOSPlatform("windows")]
    internal class CustomerClass {
        public string Name { get; set; }
        public MachineClass Machine { get; set; }

        public CustomerClass() {
            while (true) {
                Console.Write("Enter a customer name : ");
                Name = Console.ReadLine();
                if (Name != null) {
                    break;
                }
                Console.WriteLine("Please enter a proper value.");
            }
            Machine = new MachineClass();
        }

        public void LogInfo() {
            Console.WriteLine($"Client name: {Name}");
            Machine.LogInfo();
        }
    }
}
