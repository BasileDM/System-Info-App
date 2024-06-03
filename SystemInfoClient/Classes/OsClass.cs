using System.Runtime.Versioning;
using SystemInfoClient.Services;

namespace SystemInfoClient.Classes {

    public class OsClass {
        public string Directory { get; set; }
        public string Architecture { get; set; }
        public string Version { get; set; }

        // Friendly OS info
        public string? ProductName { get; set; }
        public string? ReleaseId { get; set; }
        public string? CurrentBuild { get; set; }
        public string? Ubr { get; set; }

        [SupportedOSPlatform("windows")]
        public OsClass() {
            Directory = Environment.SystemDirectory;
            Architecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits";
            Version = Environment.OSVersion.ToString();

            ProductName = RegistryService.GetRegistryValue(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "ProductName",
                "Unknown Product");
            ReleaseId = RegistryService.GetRegistryValue(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "ReleaseId",
                "Unknown Release");
            CurrentBuild = RegistryService.GetRegistryValue(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "CurrentBuild",
                "Unknown Build");
            Ubr = RegistryService.GetRegistryValue(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "UBR",
                "Unknown UBR");
        }

        public void LogInfo() {
            Console.WriteLine($"  OS System Directory: {Directory}");
            Console.WriteLine($"  OS Architecture: {Architecture}");
            Console.WriteLine($"  OS Version: {Version}");
            Console.WriteLine($"  Product Name: {ProductName}");
            Console.WriteLine($"  Release ID: {ReleaseId}");
            Console.WriteLine($"  Current Build: {CurrentBuild}");
            Console.WriteLine($"  UBR: {Ubr}");
        }
    }
}
