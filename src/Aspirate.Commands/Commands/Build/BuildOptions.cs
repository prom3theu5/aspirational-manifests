namespace Aspirate.Commands.Commands.Build;

public sealed class BuildOptions : BaseCommandOptions, IBuildOptions, IContainerOptions, IAspireOptions, IComponentsOptions
{
    public string? ProjectPath { get; set; }
    public string? AspireManifest { get; set; }
    public string? ContainerBuilder { get; set; }
    public string? ContainerBuildContext { get; set; }
    public string? ContainerRegistry { get; set; }
    public List<string>? ContainerBuildArgs { get; set; }
    public string? ContainerRepositoryPrefix { get; set; }
    public List<string>? ContainerImageTags { get; set; }
    public string? RuntimeIdentifier { get; set; }
    public List<string>? ComposeBuilds { get; set; }
    public bool PreferDockerfile { get; set; }
    public List<string>? CliSpecifiedComponents { get; set; }
}
