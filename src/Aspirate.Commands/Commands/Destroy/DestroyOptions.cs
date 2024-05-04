namespace Aspirate.Commands.Commands.Destroy;

public sealed class DestroyOptions : BaseCommandOptions, IKubernetesOptions
{
    public string? InputPath { get; set; }
    public string? KubeContext { get; set; }
}
