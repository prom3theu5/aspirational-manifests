namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Parameters;

[ExcludeFromCodeCoverage]
public class ParameterInput
{
    [JsonPropertyName("default")]
    public ParameterDefault? Default { get; set; }

    [JsonPropertyName("secret")]
    public bool Secret { get; set; }
}
