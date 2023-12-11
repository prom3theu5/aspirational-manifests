namespace Aspirate.Commands.Commands.Init;

public sealed class InitOptions : BaseCommandOptions, IInitOptions
{
    public string? ProjectPath { get; set; }
    public string? ContainerRegistry { get; set; }
    public string? ContainerImageTag { get; set; }
    public string? TemplatePath { get; set; }
}
