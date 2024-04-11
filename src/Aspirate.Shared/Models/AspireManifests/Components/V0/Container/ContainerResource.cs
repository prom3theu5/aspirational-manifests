namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Container;

public class ContainerResource : Resource, IResourceWithBinding, IResourceWithConnectionString, IResourceWithArgs, IResourceWithAnnotations, IResourceWithEnvironmentalVariables
{
    [JsonPropertyName("image")]
    public required string Image { get; set; }

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
}
