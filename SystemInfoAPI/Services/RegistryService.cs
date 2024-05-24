using Microsoft.Win32;
using System.Runtime.Versioning;

namespace SystemInfoAPI.Services {
    public class RegistryService {

        [SupportedOSPlatform("windows")]
        public static string? GetRegistryValue(string keyPath, string valueName) {
            try {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyPath);
                return key?.GetValue(valueName)?.ToString();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error reading registry: {ex.Message}");
                return null;
            }
        }
    }
}
