namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Container;

public class ContainerResource : Resource, IResourceWithInput, IResourceWithBinding, IResourceWithConnectionString, IResourceWithArgs
{
    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("bindings")]
    public Dictionary<string, Binding>? Bindings { get; set; }

    [JsonPropertyName("connectionString")]
    public string? ConnectionString { get; set; }

    [JsonPropertyName("inputs")]
    public Dictionary<string, Input>? Inputs { get; set; }

    [JsonPropertyName("args")]
    public List<string>? Args { get; set; }
}
