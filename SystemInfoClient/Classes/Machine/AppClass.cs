using System.Diagnostics;

namespace SystemInfoClient.Classes.System
{
    public class AppClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Comments { get; set; }
        public string? CompanyName { get; set; }
        public int FileBuildPart { get; set; }
        public string? FileDescription { get; set; }
        public int FileMajorPart { get; set; }
        public int FileMinorPart { get; set; }
        public string FileName { get; set; }
        public int FilePrivatePart { get; set; }
        public string? FileVersion { get; set; }
        public string? InternalName { get; set; }
        public bool IsDebug { get; set; }
        public bool IsPatched { get; set; }
        public bool IsPrivateBuild { get; set; }
        public bool IsPreRelease { get; set; }
        public bool IsSpecialBuild { get; set; }
        public string? Language { get; set; }
        public string? LegalCopyright { get; set; }
        public string? LegalTrademarks { get; set; }
        public string? OriginalFilename { get; set; }
        public string? PrivateBuild { get; set; }
        public int ProductBuildPart { get; set; }
        public int ProductMajorPart { get; set; }
        public int ProductMinorPart { get; set; }
        public string? ProductName { get; set; }
        public int ProductPrivatePart { get; set; }
        public string? ProductVersion { get; set; }
        public string? SpecialBuild { get; set; }

        public AppClass(KeyValuePair<string, ApplicationSettings> appSettings)
        {
            try
            {
                Id = appSettings.Value.ParsedId;
                Name = appSettings.Key;

                if (appSettings.Value.Path != null && File.Exists(appSettings.Value.Path))
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(appSettings.Value.Path);
                    Comments = fileVersionInfo.Comments;
                    CompanyName = fileVersionInfo.CompanyName;
                    FileBuildPart = fileVersionInfo.FileBuildPart;
                    FileDescription = fileVersionInfo.FileDescription;
                    FileMajorPart = fileVersionInfo.FileMajorPart;
                    FileMinorPart = fileVersionInfo.FileMinorPart;
                    FileName = fileVersionInfo.FileName;
                    FilePrivatePart = fileVersionInfo.FilePrivatePart;
                    FileVersion = fileVersionInfo.FileVersion;
                    InternalName = fileVersionInfo.InternalName;
                    IsDebug = fileVersionInfo.IsDebug;
                    IsPatched = fileVersionInfo.IsPatched;
                    IsPreRelease = fileVersionInfo.IsPreRelease;
                    IsPrivateBuild = fileVersionInfo.IsPrivateBuild;
                    IsSpecialBuild = fileVersionInfo.IsSpecialBuild;
                    Language = fileVersionInfo.Language;
                    LegalCopyright = fileVersionInfo.LegalCopyright;
                    LegalTrademarks = fileVersionInfo.LegalTrademarks;
                    OriginalFilename = fileVersionInfo.OriginalFilename;
                    PrivateBuild = fileVersionInfo.PrivateBuild;
                    ProductBuildPart = fileVersionInfo.ProductBuildPart;
                    ProductMajorPart = fileVersionInfo.ProductMajorPart;
                    ProductMinorPart = fileVersionInfo.ProductMinorPart;
                    ProductName = fileVersionInfo.ProductName;
                    ProductPrivatePart = fileVersionInfo.ProductPrivatePart;
                    ProductVersion = fileVersionInfo.ProductVersion;
                    SpecialBuild = fileVersionInfo.SpecialBuild;
                }
                else
                {
                    throw new FileNotFoundException(
                        $"WARNING: File not found for {appSettings.Key}, on path: {appSettings.Value.Path}. Skipping app.");
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error instantiating the machine's applications." + ex, ex);
            }
        }
        public void LogInfo()
        {
            Console.WriteLine($"    {Id}");
            Console.WriteLine($"    {Name}");
            Console.WriteLine($"    Comments: {Comments}");
            Console.WriteLine($"    CompanyName: {CompanyName}");
            Console.WriteLine($"    FileBuildPart: {FileBuildPart}");
            Console.WriteLine($"    FileDescription: {FileDescription}");
            Console.WriteLine($"    FileMajorPart: {FileMajorPart}");
            Console.WriteLine($"    FileMinorPart: {FileMinorPart}");
            Console.WriteLine($"    FileName: {FileName}");
            Console.WriteLine($"    FilePrivatePart: {FilePrivatePart}");
            Console.WriteLine($"    FileVersion: {FileVersion}");
            Console.WriteLine($"    InternalName: {InternalName}");
            Console.WriteLine($"    IsDebug: {IsDebug}");
            Console.WriteLine($"    IsPatched: {IsPatched}");
            Console.WriteLine($"    IsPrivateBuild: {IsPrivateBuild}");
            Console.WriteLine($"    IsPreRelease: {IsPreRelease}");
            Console.WriteLine($"    IsSpecialBuild: {IsSpecialBuild}");
            Console.WriteLine($"    Language: {Language}");
            Console.WriteLine($"    LegalCopyright: {LegalCopyright}");
            Console.WriteLine($"    LegalTrademarks: {LegalTrademarks}");
            Console.WriteLine($"    OriginalFilename: {OriginalFilename}");
            Console.WriteLine($"    PrivateBuild: {PrivateBuild}");
            Console.WriteLine($"    ProductBuildPart: {ProductBuildPart}");
            Console.WriteLine($"    ProductMajorPart: {ProductMajorPart}");
            Console.WriteLine($"    ProductMinorPart: {ProductMinorPart}");
            Console.WriteLine($"    ProductName: {ProductName}");
            Console.WriteLine($"    ProductPrivatePart: {ProductPrivatePart}");
            Console.WriteLine($"    ProductVersion: {ProductVersion}");
            Console.WriteLine($"    SpecialBuild: {SpecialBuild}");
            Console.WriteLine();
        }
    }
}
