namespace Aspirate.Commands.Commands.Apply;

public interface IApplyOptions
{
    string InputPath { get; set; }

    string? KubeContext { get; set; }

    bool NonInteractive { get; set; }

    ProviderType SecretProvider { get; set; }
    string? SecretPassword { get; set; }
}
