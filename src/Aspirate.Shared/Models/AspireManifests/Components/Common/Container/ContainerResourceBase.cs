namespace Aspirate.Shared.Models.AspireManifests.Components.Common.Container;

public class ContainerResourceBase : Resource,
    IResourceWithBinding,
    IResourceWithConnectionString,
    IResourceWithArgs,
    IResourceWithAnnotations,
    IResourceWithEnvironmentalVariables,
    IResourceWithVolumes
{
    [JsonPropertyName("bindings")]
    public Dictionary<string, Binding>? Bindings { get; set; }

    [JsonPropertyName("connectionString")]
    public string? ConnectionString { get; set; }

    [JsonPropertyName("args")]
    public List<string>? Args { get; set; }

    [JsonPropertyName("annotations")]
    public Dictionary<string, string>? Annotations { get; set; } = [];

    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; set; } = [];

    [JsonPropertyName("volumes")]
    public List<Volume>? Volumes { get; set; } = [];

    [JsonPropertyName("entrypoint")]
    public string? Entrypoint { get; set; }
}
