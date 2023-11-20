namespace Aspirate.Contracts.Models.Containers;

public sealed class Properties
{
    [JsonPropertyName("ContainerRegistry")]
    public string? ContainerRegistry { get; set; }

    [JsonPropertyName("ContainerRepository")]
    public string? ContainerRepository { get; set; }

    [JsonPropertyName("ContainerImage")]
    public string? ContainerImage { get; set; }

    [JsonPropertyName("ContainerImageTag")]
    public string? ContainerImageTag { get; set; }
}
