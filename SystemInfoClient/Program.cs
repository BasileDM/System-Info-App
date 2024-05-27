using System.Net.Http.Headers;
using System.Text.Json;
using SystemInfoClient.Models;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json")); // TODO: modify
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter"); // TODO: modify

string host = "https://localhost";
Console.Write("Port ? ");
string? port = Console.ReadLine();
string apiRoute = "/Systeminfo/all";
var fullAddress = host + ":" + port + apiRoute;
Console.WriteLine(fullAddress);

var systemInfo = client.GetAsync(fullAddress).Result;
Console.WriteLine(systemInfo);

Console.ReadLine();

var repositories = await ProcessRepositoriesAsync(client);
foreach (var repository in repositories) {
    Console.WriteLine($"Name: {repository.Name}");
    Console.WriteLine($"Homepage: {repository.Homepage}");
    Console.WriteLine($"GitHub: {repository.GitHubHomeUrl}");
    Console.WriteLine($"Description: {repository.Description}");
    Console.WriteLine($"Watchers: {repository.Watchers:#,0}");
    Console.WriteLine($"Last push: {repository.LastPush}");
    Console.WriteLine();
}

static async Task<List<Repository>> ProcessRepositoriesAsync(HttpClient client) {
    await using Stream stream =
        await client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
    var repositories =
        await JsonSerializer.DeserializeAsync<List<Repository>>(stream);

    return repositories ?? [];
}