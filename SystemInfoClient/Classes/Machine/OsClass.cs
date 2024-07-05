using Microsoft.Win32;
using System.Runtime.Versioning;

namespace SystemInfoClient.Classes.System
{
    public class OsClass
    {
        public string Directory { get; set; }
        public string Architecture { get; set; }
        public string Version { get; set; }
        public string? ProductName { get; set; }
        public string? ReleaseId { get; set; }
        public string? CurrentBuild { get; set; }
        public string? Ubr { get; set; }

        [SupportedOSPlatform("windows")]
        public OsClass()
        {
            try
            {
                Directory = Environment.SystemDirectory;
                Architecture = Environment.Is64BitOperatingSystem ? "x64 - 64bits" : "x86 - 32bits";
                Version = Environment.OSVersion.ToString();

                ProductName = GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "ProductName",
                    "Unknown Product");
                ReleaseId = GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "ReleaseId",
                    "Unknown Release");
                CurrentBuild = GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "CurrentBuild",
                    "Unknown Build");
                Ubr = GetRegistryValue(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "UBR",
                    "Unknown UBR");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error instantiating the machine's operating system.", ex);
            }
        }

        public void LogInfo()
        {
            Console.WriteLine($"    OS System Directory: {Directory}");
            Console.WriteLine($"    OS Architecture: {Architecture}");
            Console.WriteLine($"    OS Version: {Version}");
            Console.WriteLine($"    Product Name: {ProductName}");
            Console.WriteLine($"    Release ID: {ReleaseId}");
            Console.WriteLine($"    Current Build: {CurrentBuild}");
            Console.WriteLine($"    UBR: {Ubr}");
        }

        [SupportedOSPlatform("windows")]
        /// <summary>Gets the registry value with a path and name.</summary>
        /// <param name="keyPath">The key path.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>
        ///   A <see cref="string"/> of the registry value or the default value if an error occured.
        /// </returns>
        public static string? GetRegistryValue(string keyPath, string valueName, string defaultValue)
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyPath);
                return key?.GetValue(valueName)?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading registry: {ex.Message}");
                return defaultValue;
            }
        }
    }
}
