namespace Aspirate.Cli.Commands.Generate;

public sealed class GenerateOptions : ICommandOptions
{
    public string ProjectPath { get; init; } = AspirateLiterals.DefaultAspireProjectPath;
    public string OutputPath { get; init; } = AspirateLiterals.DefaultOutputPath;
}
