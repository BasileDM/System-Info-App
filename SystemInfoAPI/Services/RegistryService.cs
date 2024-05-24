using Microsoft.Win32;
using System.Runtime.Versioning;

namespace SystemInfoAPI.Services {

    [SupportedOSPlatform("windows")]
    public class RegistryService {
        public static string RegistryRequest(string path, string key, string defaultValue) {
            var value = GetRegistryValue(path, key);
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

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
