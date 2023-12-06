namespace Aspirate.Commands.Commands.Destroy;

public sealed class DestroyOptions : BaseCommandOptions, IDestroyOptions
{
    public string InputPath { get; set; } = AspirateLiterals.DefaultOutputPath;
    public string? KubeContext { get; set; }
}
