namespace SystemInfoClient.Classes {
    internal class CustomerClass {
        public string Name { get; set; }
        public List<MachineClass> Machines { get; set; } = [];

        public CustomerClass() {
            while (true) {
                Console.Write("Entrer le nom du client : ");
                Name = Console.ReadLine();
                if (Name == null) {
                    break;
                }
                Console.WriteLine("Veuillez entrer une valeur");
            }
        }
    }
}
