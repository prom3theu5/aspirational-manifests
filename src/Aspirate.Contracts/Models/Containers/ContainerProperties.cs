namespace Aspirate.Contracts.Models.Containers;

[ExcludeFromCodeCoverage]
public sealed class ContainerProperties
{
    [JsonPropertyName("Properties")]
    public Properties Properties { get; set; } = new();
}
