using System.Diagnostics;
using System.Reflection;
using SystemInfoClient.Models;

namespace SystemInfoClient.Services
{
    internal class ApplicationsService
    {
        private static Dictionary<string, string?> GetExeInfo(string exePath)
        {
            Dictionary<string, string?> exeInfo = new()
            {
                ["ProductName"] = FileVersionInfo.GetVersionInfo(exePath).ProductName,
                ["ProductVersion"] = FileVersionInfo.GetVersionInfo(exePath).ProductVersion,
                ["ProductMajorPart"] = FileVersionInfo.GetVersionInfo(exePath).ProductMajorPart.ToString(),
                ["ProductMinorPart"] = FileVersionInfo.GetVersionInfo(exePath).ProductMinorPart.ToString(),
                ["ProductBuildPart"] = FileVersionInfo.GetVersionInfo(exePath).ProductBuildPart.ToString(),
                ["ProductPrivatePart"] = FileVersionInfo.GetVersionInfo(exePath).ProductPrivatePart.ToString(),
                ["InternalName"] = FileVersionInfo.GetVersionInfo(exePath).InternalName,
                ["OriginalFilename"] = FileVersionInfo.GetVersionInfo(exePath).OriginalFilename,
                ["FileName"] = FileVersionInfo.GetVersionInfo(exePath).FileName,
                ["FileVersion"] = FileVersionInfo.GetVersionInfo(exePath).FileVersion,
                ["FileMajorPart"] = FileVersionInfo.GetVersionInfo(exePath).FileMajorPart.ToString(),
                ["FileMinorPart"] = FileVersionInfo.GetVersionInfo(exePath).FileMinorPart.ToString(),
                ["FileBuildPart"] = FileVersionInfo.GetVersionInfo(exePath).FileBuildPart.ToString(),
                ["FilePrivatePart"] = FileVersionInfo.GetVersionInfo(exePath).FilePrivatePart.ToString(),
                ["FileDescription"] = FileVersionInfo.GetVersionInfo(exePath).FileDescription,
                ["CompanyName"] = FileVersionInfo.GetVersionInfo(exePath).CompanyName,
                ["Language"] = FileVersionInfo.GetVersionInfo(exePath).Language,
                ["SpecialBuild"] = FileVersionInfo.GetVersionInfo(exePath).SpecialBuild,
                ["LegalCopyright"] = FileVersionInfo.GetVersionInfo(exePath).LegalCopyright,
                ["LegalTrademarks"] = FileVersionInfo.GetVersionInfo(exePath).LegalTrademarks,
                ["Comments"] = FileVersionInfo.GetVersionInfo(exePath).Comments

            };
            return exeInfo;
        }

        public static void LogAppListInfo(Dictionary<string, ApplicationSettings> appList)
        {
            foreach(KeyValuePair<string, ApplicationSettings> applicationConfig in appList)
            {
                if (File.Exists(applicationConfig.Value.Path))
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(applicationConfig.Value.Path);
                    Console.WriteLine(applicationConfig.Key);
                    foreach (PropertyInfo property in typeof(FileVersionInfo).GetProperties())
                    {
                        object? value = property.GetValue(fileVersionInfo);
                        Console.WriteLine($"{property.Name} : {value}");
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("File not found.");
                }
            }
        }
    }
}
