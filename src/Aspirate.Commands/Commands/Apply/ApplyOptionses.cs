using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyOptionses : BaseCommandOptions, IKubernetesOptions, ISecretOptions, IApplyOptions
{
    public string? InputPath { get; set; }
    public string? KubeContext { get; set; }
    public string? SecretPassword { get; set; }

    public bool RollingRestart { get; set; }
}
