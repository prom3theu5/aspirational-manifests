namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyOptions : BaseCommandOptions, IKubernetesOptions, IApplyOptions, IMinikubeOptions
{
    public string? InputPath { get; set; }
    public string? KubeContext { get; set; }
    public bool? RollingRestart { get; set; }
    public bool? EnableMinikubeMountAction { get; set; }
}
