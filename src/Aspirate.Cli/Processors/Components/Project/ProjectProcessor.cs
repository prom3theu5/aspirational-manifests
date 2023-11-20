using AspireProject = Aspirate.Contracts.Models.AspireManifests.Components.V0.Project;

namespace Aspirate.Cli.Processors.Components.Project;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public partial class ProjectProcessor(
    IFileSystem fileSystem,
    IContainerDetailsService containerDetailsService,
    ILogger<ProjectProcessor> logger)
        : BaseProcessor<ProjectTemplateData>(fileSystem, logger)
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Project;

    private readonly IReadOnlyCollection<string> _manifests =
    [
        "deployment.yml",
        "service.yml",
    ];

    /// <inheritdoc />
    public override Resource? Deserialize(ref Utf8JsonReader reader) =>
        JsonSerializer.Deserialize<AspireProject>(ref reader);

    public override async Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath)
    {
        var resourceOutputPath = Path.Combine(outputPath, resource.Key);

        LogHandlerExecution(logger, nameof(ProjectProcessor), resourceOutputPath);

        EnsureOutputDirectoryExistsAndIsClean(resourceOutputPath);

        var project = resource.Value as AspireProject;

        var containerDetails = await containerDetailsService.GetContainerDetails(resource.Key, project);

        ArgumentNullException.ThrowIfNull(containerDetails, nameof(containerDetails));

        var data = new ProjectTemplateData(
            resource.Key,
            containerDetailsService.GetFullImage(containerDetails, resource.Key),
            project.Env,
            _manifests);

        AugmentData(data);

        CreateDeployment(resourceOutputPath, data);
        CreateService(resourceOutputPath, data);
        CreateComponentKustomizeManifest(resourceOutputPath, data);

        return true;
    }

    private static void AugmentData(ProjectTemplateData data)
    {
        RemoveTlsServiceMappingForNow(data.Env);
        MapCorrectServiceAddressesForDeployment(data.Env);
        MapCache(data.Env);
        MapPostgres(data.Env);
    }


    // TODO: Handle the port once TLS is supported from the Binging value set in the value prior to augmentation.
    private static void MapCorrectServiceAddressesForDeployment(Dictionary<string, string>? env)
    {
        foreach (var key in env.Keys.ToList())
        {
            if (key.StartsWith("services__"))
            {
                ReadOnlySpan<char> span = env[key].AsSpan();
                int start = span.IndexOf('{') + 1;
                int end = span.IndexOf('.');
                string serviceName = span[start..end].ToString();
                env[key] = $"http://{serviceName}:8080";
            }
        }
    }

    // TODO: Handle this once service types are included in bindings maybe?
    private static void MapCache(Dictionary<string, string>? env)
    {
        foreach (var key in env.Keys.ToList())
        {
            if (key.AsSpan()[^5..].ToString() == "cache")
            {
                env[key] = "redis";
            }
        }
    }

    // TODO: Handle this once service types are included in bindings maybe?
    private static void MapPostgres(Dictionary<string, string>? env)
    {
        foreach (var key in env.Keys.ToList())
        {
            if (key.AsSpan()[^2..].ToString() == "db")
            {
                env[key] = "host=postgres-service;username=postgres;password=postgres;database=catalogdb";
            }
        }
    }

    // TODO: Remove this once TLS is supported.
    private static void RemoveTlsServiceMappingForNow(Dictionary<string, string>? env)
    {
        foreach (var key in env.Keys.ToList())
        {
            if (key.AsSpan()[^3..].ToString() == "__1")
            {
                env.Remove(key);
            }
        }
    }
}


