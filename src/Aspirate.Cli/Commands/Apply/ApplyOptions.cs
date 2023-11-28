namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyOptions : BaseCommandOptions
{
    public string InputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
    public string? KubeContext { get; set; }
}
