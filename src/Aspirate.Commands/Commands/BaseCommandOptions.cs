namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptions : ICommandOptions
{
    public bool NonInteractive { get; set; }
    public ProviderType SecretProvider { get; set; }

    public bool DisableSecrets { get; set; }
}
