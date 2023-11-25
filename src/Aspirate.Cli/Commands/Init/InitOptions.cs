namespace Aspirate.Cli.Commands.Init;

public sealed class InitOptions : ICommandOptions
{
    public string ProjectPath { get; init; } = AspirateLiterals.DefaultAspireProjectPath;
}
