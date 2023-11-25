namespace Aspirate.Cli.Commands.Destroy;

public sealed class DestroyOptions : ICommandOptions
{
    public string InputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
}
