namespace Aspirate.Cli.Commands.Generate;

public sealed class GenerateOptions : ICommandOptions
{
    public string ProjectPath { get; set; } = AspirateLiterals.DefaultAspireProjectPath;
    public string? AspireManifest { get; set; }
    public string OutputPath { get; set; } = AspirateLiterals.DefaultOutputPath;

    public bool SkipBuild { get; set; } = false;
    public bool NonInteractive { get; set; } = false;
}
