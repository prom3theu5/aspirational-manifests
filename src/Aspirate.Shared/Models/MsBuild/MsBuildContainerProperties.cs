namespace Aspirate.Shared.Models.MsBuild;

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

    [JsonPropertyName(MsBuildPropertiesLiterals.DockerfileFileArgument)]
    public string? DockerfileFile { get; set; }

    [JsonPropertyName(MsBuildPropertiesLiterals.DockerfileContextArgument)]
    public string? DockerfileContext { get; set; }

    [JsonIgnore]
    public string? FullContainerImage { get; set; }
}
