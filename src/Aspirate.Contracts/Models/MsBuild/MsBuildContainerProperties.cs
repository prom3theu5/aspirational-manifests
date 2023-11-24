namespace Aspirate.Contracts.Models.MsBuild;

[ExcludeFromCodeCoverage]
public sealed class MsBuildContainerProperties : BaseMsBuildProperties
{
    [JsonPropertyName(MsBuildPropertiesLiterals.ContainerRegistryArgument)]
    public string? ContainerRegistry { get; set; }

    [JsonPropertyName(MsBuildPropertiesLiterals.ContainerRepositoryArgument)]
    public string? ContainerRepository { get; set; }

    [JsonPropertyName(MsBuildPropertiesLiterals.ContainerImageNameArgument)]
    public string? ContainerImageName { get; set; }

    [JsonPropertyName(MsBuildPropertiesLiterals.ContainerImageTagArgument)]
    public string? ContainerImageTag { get; set; }

    [JsonIgnore]
    public string? FullContainerImage { get; set; }
}
