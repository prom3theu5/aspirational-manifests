namespace Aspirate.Cli.Commands.Build;

public sealed class BuildOptions : ICommandOptions
{
    public string ProjectPath { get; set; } = AspirateLiterals.DefaultAspireProjectPath;
    public string? AspireManifest { get; set; }
    public bool NonInteractive { get; set; } = false;
}
