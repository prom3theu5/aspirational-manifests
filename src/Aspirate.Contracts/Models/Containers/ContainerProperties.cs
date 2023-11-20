namespace Aspirate.Contracts.Models.Containers;

public sealed class ContainerProperties
{
    [JsonPropertyName("Properties")]
    public Properties Properties { get; set; } = new();
}
