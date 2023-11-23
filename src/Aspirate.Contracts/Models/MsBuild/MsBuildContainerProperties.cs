namespace Aspirate.Contracts.Models.MsBuild;

[ExcludeFromCodeCoverage]
public sealed class MsBuildContainerProperties : BaseMsBuildProperties
{
    [JsonPropertyName("ContainerRegistry")]
    public string? ContainerRegistry { get; set; }

    [JsonPropertyName("ContainerRepository")]
    public string? ContainerRepository { get; set; }

    [JsonPropertyName("ContainerImage")]
    public string? ContainerImage { get; set; }

    [JsonPropertyName("ContainerImageTag")]
    public string? ContainerImageTag { get; set; }

    [JsonIgnore]
    public string? FullContainerImage { get; set; }
}
