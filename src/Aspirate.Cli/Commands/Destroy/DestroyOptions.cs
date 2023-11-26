namespace Aspirate.Cli.Commands.Destroy;

public sealed class DestroyOptions : ICommandOptions
{
    public string InputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
    public string? KubeContext { get; set; }
    public bool NonInteractive { get; set; } = false;
}
