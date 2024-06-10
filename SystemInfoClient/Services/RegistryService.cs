using Microsoft.Win32;
using System.Runtime.Versioning;

namespace SystemInfoClient.Services
{
    [SupportedOSPlatform("windows")]
    internal class RegistryService
    {
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