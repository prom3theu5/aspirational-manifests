namespace Aspirate.Cli.Commands.Build;

public sealed class BuildOptions : BaseCommandOptions
{
    public string ProjectPath { get; set; } = AspirateLiterals.DefaultAspireProjectPath;
    public string? AspireManifest { get; set; }

    public string? ContainerBuilder { get; set; } = "docker";
}
