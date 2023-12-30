namespace Aspirate.Commands.Commands;

public interface ICommandOptions
{
    bool NonInteractive { get; set; }
    ProviderType SecretProvider { get; set; }
    bool DisableSecrets { get; set; }
}
