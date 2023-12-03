namespace Aspirate.Commands.Commands.Build;

public sealed class BuildOptions : BaseCommandOptions, IBuildOptions
{
    public string ProjectPath { get; set; } = AspirateLiterals.DefaultAspireProjectPath;
    public string? AspireManifest { get; set; }

    public string? ContainerBuilder { get; set; } = "docker";
    public string? ContainerRegistry { get; set; }
    public string? ContainerImageTag { get; set; }
}
