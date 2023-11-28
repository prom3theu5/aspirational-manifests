namespace Aspirate.Cli.Commands.Init;

public sealed class InitOptions : BaseCommandOptions
{
    public string ProjectPath { get; set; } = AspirateLiterals.DefaultAspireProjectPath;
    public string? ContainerRegistry { get; set; }
    public string? ContainerImageTag { get; set; }
    public string? TemplatePath { get; set; }
}
