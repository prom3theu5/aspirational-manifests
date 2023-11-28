namespace Aspirate.Cli.Commands.Destroy;

public sealed class DestroyOptions : BaseCommandOptions
{
    public string InputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
    public string? KubeContext { get; set; }
}
