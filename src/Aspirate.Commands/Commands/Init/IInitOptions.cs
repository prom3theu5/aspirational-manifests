namespace Aspirate.Commands.Commands.Init;

public interface IInitOptions
{
    string ProjectPath { get; set; }

    string? ContainerRegistry { get; set; }

    string? ContainerImageTag { get; set; }

    string? TemplatePath { get; set; }

    bool NonInteractive { get; set; }

    ProviderType SecretProvider { get; set; }
}
