using System.Runtime.Versioning;
using SystemInfoClient.Services;

namespace SystemInfoClient.Classes {

    public class OsClass {
        public string? SystemDirectory { get; set; }
        public string? OsArchitecture { get; set; }
        public string? OsVersion { get; set; }

        // Friendly OS info
        public string? ProductName { get; set; }
        public string? ReleaseId { get; set; }
        public string? CurrentBuild { get; set; }
        public string? Ubr { get; set; }

        [SupportedOSPlatform("windows")]
        public OsClass() {

            SystemDirectory = Environment.SystemDirectory;
            OsArchitecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits";
            OsVersion = Environment.OSVersion.ToString();

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
    }
}
