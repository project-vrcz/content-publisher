#!/usr/bin/dotnet run

#:package System.CommandLine@2.0.6

using System.CommandLine;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

var ownerOption = new Option<string>("--owner")
{
    Description = "GitHub repository owner",
    Required = true
};

var repoOption = new Option<string>("--repo")
{
    Description = "GitHub repository name",
    Required = true
};

var sourceOption = new Option<ReleaseSource>("--source")
{
    Description = "Release source selector: latest, prerelease, specified",
    DefaultValueFactory = _ => ReleaseSource.Latest
};

var tagOrNameOption = new Option<string?>("--tag-or-name")
{
    Description = "Tag or release name used when --source specified"
};

var githubTokenOption = new Option<string?>("--github-token")
{
    Description = "GitHub token (overrides GITHUB_TOKEN environment variable)"
};

var outputOption = new Option<FileInfo?>("--output")
{
    Description = "Write updater json to file. If omitted, output will be printed to stdout"
};

var dryRunOption = new Option<bool>("--dry-run")
{
    Description = "Skip asset download and SHA256 calculation, useful for quick validation",
    DefaultValueFactory = _ => false
};

var rootCommand = new RootCommand("Generate updater metadata from GitHub release");
rootCommand.Options.Add(ownerOption);
rootCommand.Options.Add(repoOption);
rootCommand.Options.Add(sourceOption);
rootCommand.Options.Add(tagOrNameOption);
rootCommand.Options.Add(githubTokenOption);
rootCommand.Options.Add(outputOption);
rootCommand.Options.Add(dryRunOption);

rootCommand.SetAction(async parseResult =>
{
    try
    {
        var owner = parseResult.GetRequiredValue(ownerOption);
        var repo = parseResult.GetRequiredValue(repoOption);
        var source = parseResult.GetRequiredValue(sourceOption);
        var tagOrName = parseResult.GetValue(tagOrNameOption);
        var githubToken = parseResult.GetValue(githubTokenOption);
        var outputFile = parseResult.GetValue(outputOption);
        var dryRun = parseResult.GetRequiredValue(dryRunOption);

        if (source == ReleaseSource.Specified && string.IsNullOrWhiteSpace(tagOrName))
            throw new ArgumentException("--tag-or-name is required when --source specified");

        githubToken = string.IsNullOrWhiteSpace(githubToken)
            ? Environment.GetEnvironmentVariable("GITHUB_TOKEN")
            : githubToken;

        using var httpClient = CreateGitHubApiClient(githubToken);

        var release = await GetReleaseAsync(httpClient, owner, repo, source, tagOrName);
        var metadata = await BuildUpdaterMetadataAsync(httpClient, release, dryRun);

        var json = JsonSerializer.Serialize(metadata, UpdaterMetadataJsonContext.Default.AppUpdateInformation);

        if (outputFile is null)
        {
            Console.WriteLine(json);
            return;
        }

        var outputDirectory = outputFile.DirectoryName;
        if (!string.IsNullOrWhiteSpace(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        await File.WriteAllTextAsync(outputFile.FullName, json);
        Console.Error.WriteLine($"Generated updater metadata: {outputFile.FullName}");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.ExitCode = 1;
    }
});

return await rootCommand.Parse(args).InvokeAsync();

static HttpClient CreateGitHubApiClient(string? token)
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("https://api.github.com/")
    };
    client.DefaultRequestHeaders.UserAgent.ParseAdd("VRChatContentManager-UpdaterMetadataGenerator");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

    if (!string.IsNullOrWhiteSpace(token))
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    return client;
}

static async Task<AppUpdateInformation> BuildUpdaterMetadataAsync(HttpClient httpClient, GitHubRelease release, bool dryRun)
{
    var winX64Asset = release.Assets.FirstOrDefault(asset =>
        asset.Name.EndsWith("app-win-x64-installer.zip", StringComparison.OrdinalIgnoreCase));

    if (winX64Asset is null)
        throw new InvalidOperationException("No win-x64 release asset found.");

    var sha256 = dryRun
        ? "dry-run-skip-sha256"
        : await ComputeSha256FromUrlAsync(httpClient, winX64Asset.BrowserDownloadUrl);

    var platforms = new Dictionary<string, AppUpdatePlatformInformation>
    {
        ["win-x64"] = new AppUpdatePlatformInformation(winX64Asset.BrowserDownloadUrl, sha256)
    };

    var version = release.TagName.TrimStart('v', 'V');
    var releaseDate = release.PublishedAt ?? release.CreatedAt;

    return new AppUpdateInformation(
        Version: version,
        Notes: release.Body ?? string.Empty,
        BrowserUrl: release.HtmlUrl,
        ReleaseDate: releaseDate,
        Platforms: platforms
    );
}

static async Task<string> ComputeSha256FromUrlAsync(HttpClient httpClient, string assetUrl)
{
    using var request = new HttpRequestMessage(HttpMethod.Get, assetUrl);
    using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

    if (!response.IsSuccessStatusCode)
    {
        throw new InvalidOperationException(
            $"Unable to download release asset for hash calculation: {(int)response.StatusCode} {response.ReasonPhrase}");
    }

    await using var stream = await response.Content.ReadAsStreamAsync();
    var hash = await SHA256.HashDataAsync(stream);
    return Convert.ToHexString(hash).ToLowerInvariant();
}

static async Task<GitHubRelease> GetReleaseAsync(
    HttpClient httpClient,
    string owner,
    string repo,
    ReleaseSource source,
    string? tagOrName
)
{
    return source switch
    {
        ReleaseSource.Latest => await GetLatestReleaseAsync(httpClient, owner, repo),
        ReleaseSource.Prerelease => await GetLatestPrereleaseAsync(httpClient, owner, repo),
        ReleaseSource.Specified => await GetSpecifiedReleaseAsync(httpClient, owner, repo, tagOrName!),
        _ => throw new ArgumentOutOfRangeException(nameof(source), source, "Unknown release source")
    };
}

static async Task<GitHubRelease> GetLatestReleaseAsync(HttpClient httpClient, string owner, string repo)
{
    using var response = await httpClient.GetAsync($"repos/{owner}/{repo}/releases/latest");
    if (response.StatusCode == HttpStatusCode.NotFound)
        throw new InvalidOperationException("Unable to find latest stable release.");

    var release = await ReadReleaseResponseAsync(response);
    if (release.Draft || release.Prerelease)
        throw new InvalidOperationException("GitHub latest endpoint returned non-stable release.");

    return release;
}

static async Task<GitHubRelease> GetLatestPrereleaseAsync(HttpClient httpClient, string owner, string repo)
{
    var releases = await GetReleasesAsync(httpClient, owner, repo);
    var release = releases.FirstOrDefault(item => !item.Draft && item.Prerelease);

    return release
           ?? throw new InvalidOperationException("Unable to find prerelease.");
}

static async Task<GitHubRelease> GetSpecifiedReleaseAsync(
    HttpClient httpClient,
    string owner,
    string repo,
    string tagOrName
)
{
    var byTag = await TryGetReleaseByTagAsync(httpClient, owner, repo, tagOrName);
    if (byTag is not null)
        return byTag;

    var releases = await GetReleasesAsync(httpClient, owner, repo);
    var byName = releases.FirstOrDefault(item =>
        !item.Draft
        && !string.IsNullOrWhiteSpace(item.Name)
        && string.Equals(item.Name, tagOrName, StringComparison.OrdinalIgnoreCase));

    return byName
           ?? throw new InvalidOperationException($"Unable to find release by tag or name: {tagOrName}");
}

static async Task<GitHubRelease?> TryGetReleaseByTagAsync(
    HttpClient httpClient,
    string owner,
    string repo,
    string tag
)
{
    using var response = await httpClient.GetAsync($"repos/{owner}/{repo}/releases/tags/{Uri.EscapeDataString(tag)}");
    if (response.StatusCode == HttpStatusCode.NotFound)
        return null;

    return await ReadReleaseResponseAsync(response);
}

static async Task<IReadOnlyList<GitHubRelease>> GetReleasesAsync(HttpClient httpClient, string owner, string repo)
{
    using var response = await httpClient.GetAsync($"repos/{owner}/{repo}/releases?per_page=100");
    var releases = await ReadReleasesResponseAsync(response);

    return releases;
}

static async Task<GitHubRelease> ReadReleaseResponseAsync(HttpResponseMessage response)
{
    var body = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
    {
        throw new InvalidOperationException(
            $"GitHub API request failed: {(int)response.StatusCode} {response.ReasonPhrase}. {body}");
    }

    var result = JsonSerializer.Deserialize(body, UpdaterMetadataJsonContext.Default.GitHubRelease);
    return result ?? throw new InvalidOperationException("Unable to deserialize GitHub API response.");
}

static async Task<IReadOnlyList<GitHubRelease>> ReadReleasesResponseAsync(HttpResponseMessage response)
{
    var body = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
    {
        throw new InvalidOperationException(
            $"GitHub API request failed: {(int)response.StatusCode} {response.ReasonPhrase}. {body}");
    }

    var result = JsonSerializer.Deserialize(body, UpdaterMetadataJsonContext.Default.ListGitHubRelease);
    return result ?? throw new InvalidOperationException("Unable to deserialize GitHub API response.");
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
    WriteIndented = true,
    GenerationMode = JsonSourceGenerationMode.Metadata
)]
[JsonSerializable(typeof(AppUpdateInformation))]
[JsonSerializable(typeof(GitHubRelease))]
[JsonSerializable(typeof(List<GitHubRelease>))]
internal partial class UpdaterMetadataJsonContext : JsonSerializerContext
{
}

public sealed record AppUpdateInformation(
    [property: JsonPropertyName("version")]
    string Version,
    [property: JsonPropertyName("notes")] string Notes,
    [property: JsonPropertyName("browserUrl")]
    string BrowserUrl,
    [property: JsonPropertyName("releaseDate")]
    DateTimeOffset ReleaseDate,
    [property: JsonPropertyName("platforms")]
    Dictionary<string, AppUpdatePlatformInformation> Platforms
);

public sealed record AppUpdatePlatformInformation(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("sha256")] string Sha256
);

public enum ReleaseSource
{
    Latest,
    Prerelease,
    Specified
}

public sealed record GitHubRelease(
    [property: JsonPropertyName("tag_name")] string TagName,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("body")] string? Body,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("published_at")]
    DateTimeOffset? PublishedAt,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("prerelease")] bool Prerelease,
    [property: JsonPropertyName("draft")] bool Draft,
    [property: JsonPropertyName("assets")]
    IReadOnlyList<GitHubReleaseAsset> Assets
);

public sealed record GitHubReleaseAsset(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("browser_download_url")] string BrowserDownloadUrl
);