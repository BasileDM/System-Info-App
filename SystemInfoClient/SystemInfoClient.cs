using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Runtime.Versioning;
using SystemInfoClient.Classes;
using System.Diagnostics;

namespace SystemInfoClient;

internal class SystemInfoClient {

    [SupportedOSPlatform("windows")]
    public static async Task Main(string[] args) {

        var customer = new CustomerClass();
        //customer.LogInfo();

        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("User-Agent", "Systeminfo App Client");

        string protocol = "https";
        string host = "localhost";
        string port = "7056";
        string route = "/Systeminfo/drives";

        var fullAddress = protocol + "://" + host + ":" + port + route;

        Console.WriteLine($"Posting at: {fullAddress}");
        Console.WriteLine();

        var drives = await ProcessDrivesAsync(client, fullAddress);

        foreach (var drive in drives) {
            Console.WriteLine($"Name: {drive.Name}");
            Console.WriteLine($"Label: {drive.Label}");
            Console.WriteLine($"Drive type: {drive.DriveType}");
            Console.WriteLine($"Drive format: {drive.DriveFormat}");
            Console.WriteLine($"Total size: {drive.TotalSize:#,0} bits");
            Console.WriteLine($"Available space: {drive.AvailableFreeSpace:#,0} bits");
            Console.WriteLine($"Total free space: {drive.TotalFreeSpace:#,0} bits");
            Console.WriteLine($"Free space percentage: {drive.SpacePercentageStr}");
            Console.WriteLine();
        }
        Console.ReadLine();

        // Async methods

        static async Task<List<DriveModel>> ProcessDrivesAsync(HttpClient client, string address) {
            await using Stream stream =
                await client.GetStreamAsync(address);
            var drives =
                await JsonSerializer.DeserializeAsync<List<DriveModel>>(stream);

            return drives ?? [];
        }

    }

    public static string? FindVsVersion() {
        string exePath = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe";
        string? version = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
        return version;
    }
}