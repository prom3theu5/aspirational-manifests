namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateOptions : BaseCommandOptions
{
    public string ProjectPath { get; set; } = AspirateLiterals.DefaultAspireProjectPath;
    public string? AspireManifest { get; set; }
    public string OutputPath { get; set; } = AspirateLiterals.DefaultOutputPath;

    public bool SkipBuild { get; set; } = false;
    public bool SkipFinalKustomizeGeneration { get; set; } = false;
    public string? ContainerBuilder { get; set; } = "docker";
    public string? ContainerRegistry { get; set; }
    public string? ContainerImageTag { get; set; }
}
