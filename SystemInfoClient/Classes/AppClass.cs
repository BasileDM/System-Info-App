using System.Diagnostics;
using System.Reflection;
using SystemInfoClient.Models;

namespace SystemInfoClient.Classes
{
    public class AppClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> FileVersionProperties { get; set; }

        public AppClass(KeyValuePair<string, ApplicationSettings> app)
        {
            if (Int32.TryParse(app.Value.Id, out int parsedId) && parsedId >= 0)
            {
                Id = parsedId;
            }
            else
            {
                throw new InvalidOperationException($"Invalid configuration for {app.Value}'s ID in settings.json.");
            }

            Name = app.Key;
            FileVersionProperties = [];

            if (File.Exists(app.Value.Path) && app.Value.Path != null)
            {
                FileVersionInfo filVersionInfo = FileVersionInfo.GetVersionInfo(app.Value.Path);
                foreach (PropertyInfo property in typeof(FileVersionInfo).GetProperties())
                {
                    object? value = property.GetValue(filVersionInfo);
                    FileVersionProperties.Add(property.Name, value);
                }
            }
            else
            {
                throw new FileNotFoundException($"File not found for {app.Value} with path {app.Value.Path}");
            }

        }

        public void LogInfo()
        {
            Console.WriteLine($"    {Id}");
            Console.WriteLine($"    {Name}");
            foreach (var entry in FileVersionProperties)
            {
                Console.WriteLine($"    {entry.Key} : {entry.Value}");
            }
            Console.WriteLine();
        }
    }
}
