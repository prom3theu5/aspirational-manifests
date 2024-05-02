namespace Aspirate.Shared.Interfaces.Commands;

public interface ICommandOptions
{
    bool NonInteractive { get; set; }
    SecretProviderType SecretProvider { get; set; }
    bool DisableSecrets { get; set; }
}
