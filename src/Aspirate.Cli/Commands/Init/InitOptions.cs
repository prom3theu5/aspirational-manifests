namespace Aspirate.Cli.Commands.Init;

public sealed class InitOptions : ICommandOptions
{
    public string ProjectPath { get; set; } = AspirateLiterals.DefaultAspireProjectPath;
    public string? ContainerRegistry { get; set; }
    public string? ContainerImageTag { get; set; }
    public string? TemplatePath { get; set; }
    public bool NonInteractive { get; set; } = false;
}
