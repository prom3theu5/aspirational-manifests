namespace Aspirate.Commands.Commands.Build;

public interface IBuildOptions
{
    string? ProjectPath { get; set; }

    string? AspireManifest { get; set; }

    string? ContainerBuilder { get; set; }

    string? ContainerRegistry { get; set; }

    string? ContainerImageTag { get; set; }

    bool NonInteractive { get; set; }

    ProviderType SecretProvider { get; set; }
}
