namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface IKubernetesOptions
{
    string? InputPath { get; set; }

    string? KubeContext { get; set; }
}
