using System.Diagnostics;
using System.Reflection;

namespace SystemInfoClient.Services {
    internal class ApplicationsService {
        private static Dictionary<string, string?> GetExeInfo(string exePath) {

            Dictionary<string, string?> exeInfo = new();

            exeInfo["ProductName"] = FileVersionInfo.GetVersionInfo(exePath).ProductName;
            exeInfo["ProductVersion"] = FileVersionInfo.GetVersionInfo(exePath).ProductVersion;
            exeInfo["FileVersion"] = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
            exeInfo["OriginalFilename"] = FileVersionInfo.GetVersionInfo(exePath).OriginalFilename;
            exeInfo["FileName"] = FileVersionInfo.GetVersionInfo(exePath).FileName;
            exeInfo["FileDescription"] = FileVersionInfo.GetVersionInfo(exePath).FileDescription;
            exeInfo["CompanyName"] = FileVersionInfo.GetVersionInfo(exePath).CompanyName;
            exeInfo["Language"] = FileVersionInfo.GetVersionInfo(exePath).Language;
            exeInfo["SpecialBuild"] = FileVersionInfo.GetVersionInfo(exePath).SpecialBuild;
            exeInfo["InternalName"] = FileVersionInfo.GetVersionInfo(exePath).InternalName;
            exeInfo["LegalCopyright"] = FileVersionInfo.GetVersionInfo(exePath).LegalCopyright;

            return exeInfo;
        }

        public static void LogExeInfo(string path) {
            if (File.Exists(path)) {

                foreach (var entry in GetExeInfo(path)) {
                    Console.WriteLine($"{entry.Key} : {entry.Value}");
                }
            } 
            else {
                Console.WriteLine("File not found.");
            }
        }
    }
}
