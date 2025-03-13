namespace Aspirate.Commands.Commands.Destroy;

public sealed class DestroyOptions : BaseCommandOptions, IKubernetesOptions, IMinikubeOptions
{
    public string? InputPath { get; set; }
    public string? KubeContext { get; set; }
    public bool? EnableMinikubeMountAction { get; set; }
}
