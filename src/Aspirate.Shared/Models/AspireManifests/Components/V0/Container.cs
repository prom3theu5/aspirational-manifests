namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

public class Container : Resource, IResourceWithInput, IResourceWithBinding
{
    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("bindings")]
    public Dictionary<string, Binding>? Bindings { get; set; }

    [JsonPropertyName("connectionString")]
    public string? ConnectionString { get; set; }

    [JsonPropertyName("inputs")]
    public Dictionary<string, Input>? Inputs { get; set; }
}
