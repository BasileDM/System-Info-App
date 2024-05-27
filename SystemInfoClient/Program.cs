using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using SystemInfoClient.Models;

namespace SystemInfoClient;

internal class Program {
    private static async Task Main(string[] args) {

        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")); // TODO: modify
        client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter"); // TODO: modify

        string host = "https://localhost";
        Console.Write("Port ? ");
        string? port = Console.ReadLine();
        var fullAddress = host + ":" + port + "/Systeminfo/drives";

        Console.WriteLine($"Fetching at: {fullAddress}");
        Console.WriteLine();

        var drives = await ProcessDrivesAsync(client, fullAddress);
        foreach (var drive in drives) {
            Console.WriteLine($"Name: {drive.Name}");
            Console.WriteLine($"Label: {drive.Label}");
            Console.WriteLine($"Available space: {drive.AvailableFreeSpace:#,0} bits");
            Console.WriteLine();
        }
        Console.ReadLine();

        static async Task<List<Drive>> ProcessDrivesAsync(HttpClient client, string address) {
            await using Stream stream =
                await client.GetStreamAsync(address);
            var drives =
                await JsonSerializer.DeserializeAsync<List<Drive>>(stream);

            return drives ?? [];
        }
    }
}