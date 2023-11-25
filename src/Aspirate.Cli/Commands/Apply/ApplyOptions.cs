namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyOptions : ICommandOptions
{
    public string OutputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
}
