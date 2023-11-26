namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyOptions : ICommandOptions
{
    public string InputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
}
