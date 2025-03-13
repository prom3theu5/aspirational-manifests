namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Parameters;

[ExcludeFromCodeCoverage]
public class ParameterResource : Resource, IResourceWithInput, IResourceWithValue, IResourceWithConnectionString
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("inputs")]
    public Dictionary<string, ParameterInput>? Inputs { get; set; }

    [JsonPropertyName("connectionString")]
    public string? ConnectionString { get; set; }
}
