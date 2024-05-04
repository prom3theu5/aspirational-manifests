namespace Aspirate.Commands.Commands.Build;

public sealed class BuildOptions : BaseCommandOptions, IBuildOptions, IContainerOptions, IAspireOptions
{
    public string? ProjectPath { get; set; }
    public string? AspireManifest { get; set; }
    public string? ContainerBuilder { get; set; }
    public string? ContainerRegistry { get; set; }

    public string? ContainerRepositoryPrefix { get; set; }

    public string? ContainerImageTag { get; set; }
    public string? RuntimeIdentifier { get; set; }
    public List<string>? ComposeBuilds { get; set; }
}
