using Volume=Aspirate.Shared.Models.AspireManifests.Components.V0.Volume;

namespace Aspirate.Shared.Models.AspireManifests.Components.V1.Container;

public class ContainerV1BuildContext
{
    [JsonPropertyName("context")]
    public string Context { get; init; } = null!;

    [JsonPropertyName("dockerfile")]
    public string Dockerfile { get; init; } = null!;
}

public class ContainerV1Resource : Resource,
    IResourceWithBinding,
    IResourceWithConnectionString,
    IResourceWithArgs,
    IResourceWithAnnotations,
    IResourceWithEnvironmentalVariables,
    IResourceWithVolumes
{
    [JsonPropertyName("connectionString")]
    public string? ConnectionString { get; set; }

    [JsonPropertyName("build")]
    public ContainerV1BuildContext Build { get; set; } = null!;

    [JsonPropertyName("args")]
    public List<string>? Args { get; set; }

    [JsonPropertyName("annotations")]
    public Dictionary<string, string>? Annotations { get; set; }

    [JsonPropertyName("volumes")]
    public List<Volume>? Volumes { get; set; }

    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; set; }

    [JsonPropertyName("bindings")]
    public Dictionary<string, Binding>? Bindings { get; set; }

    [JsonPropertyName("entrypoint")]
    public string? Entrypoint { get; set; }
}
