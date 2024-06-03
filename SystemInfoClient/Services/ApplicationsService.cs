using System.Diagnostics;

namespace SystemInfoClient.Services {
    internal class ApplicationsService {
        static string? FindAppVersion(string exePath) {
            string? version = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
            return version;
            //Example path: C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe

        }
    }
}
