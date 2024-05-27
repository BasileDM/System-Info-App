using System.Net.Http.Headers;
using System.Text.Json;
using SystemInfoClient;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

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