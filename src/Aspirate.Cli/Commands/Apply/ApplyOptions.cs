namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyOptions : ICommandOptions
{
    public string InputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
    public string? KubeContext { get; set; }
    public bool NonInteractive { get; set; } = false;
}
