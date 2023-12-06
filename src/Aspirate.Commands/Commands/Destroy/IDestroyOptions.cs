namespace Aspirate.Commands.Commands.Destroy;

public interface IDestroyOptions
{
    string InputPath { get; set; }

    string? KubeContext { get; set; }

    bool NonInteractive { get; set; }

    ProviderType SecretProvider { get; set; }
}
