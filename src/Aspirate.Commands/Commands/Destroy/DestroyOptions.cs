namespace Aspirate.Commands.Commands.Destroy;

public sealed class DestroyOptions : BaseCommandOptions, IDestroyOptions
{
    public string? InputPath { get; set; }
    public string? KubeContext { get; set; }
}
