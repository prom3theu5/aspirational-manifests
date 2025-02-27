using System.Net;
using Aspirate.Shared.Models.ContainerRegistry;

namespace Aspirate.Services.Implementations;

public class ContainerRegistryV2Client
{
    private readonly HttpClient _httpClient;

    public const string
        ApplicationJsonMediaType = "application/json",
        ImageV1MediaType = "application/vnd.docker.container.image.v1+json",
        ManifestV2MediaType = "application/vnd.docker.distribution.manifest.v2+json";

    private ContainerRegistryV2Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static Uri CreateV2Uri(string containerRegistry, bool isTls) =>
        new((isTls ? "https" : "http") + $"://{containerRegistry}/v2/");

    public static async Task<ContainerRegistryV2Client> ConnectAsync(
        string containerRegistry,
#pragma warning disable IDE0060 // Remove unused parameter
        string? containerUsername = null,
        string? containerPassword = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        using var client = new HttpClient();
        var exceptions = new List<Exception>();

        foreach (var isTls in new[] { true, false })
        {
            try
            {
                var uri = CreateV2Uri(containerRegistry, isTls);
                var resp = await client.SendAsync(new(HttpMethod.Get, uri));

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    var containerClient = new HttpClient
                    {
                        BaseAddress = uri
                    };

                    return new(containerClient);
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        throw new AggregateException(exceptions);
    }

    private async Task<T> GetAsync<T>(
        string relativeUri,
        string acceptMediaType = ApplicationJsonMediaType)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, relativeUri);
        req.Headers.Accept.Add(new(acceptMediaType));

        var resp = await _httpClient.SendAsync(req);

        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(body) ??
            throw new JsonException($"Could not deserialize {typeof(T).Name} from HTTP response body.");
    }

    public async Task<RegistryCatalogV2> GetCatalogAsync() =>
        await GetAsync<RegistryCatalogV2>("_catalog");

    public async Task<RegistryTagsV2> GetTagsAsync(string repository) =>
        await GetAsync<RegistryTagsV2>($"{repository}/tags/list");

    public async Task<ImageManifestListV2> GetManifestAsync(string repository, string tag) =>
        await GetAsync<ImageManifestListV2>($"{repository}/manifests/{tag}", ManifestV2MediaType);

    public async Task<ImageV1> GetDockerImageJsonBlobAsync(string repository, string digest) =>
        await GetAsync<ImageV1>($"{repository}/blobs/{digest}", ManifestV2MediaType);
}
