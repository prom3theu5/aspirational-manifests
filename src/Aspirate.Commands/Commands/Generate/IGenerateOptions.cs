namespace Aspirate.Commands.Commands.Generate;

public interface IGenerateOptions
{
    string ProjectPath { get; set; }

    string? AspireManifest { get; set; }

    string OutputPath { get; set; }

    bool SkipBuild { get; set; }

    bool SkipFinalKustomizeGeneration { get; set; }

    string? ContainerBuilder { get; set; }

    string? ContainerRegistry { get; set; }

    string? ContainerImageTag { get; set; }

    bool NonInteractive { get; set; }

    ProviderType SecretProvider { get; set; }
}
