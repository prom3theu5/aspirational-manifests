namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptions : ICommandOptions
{
    public bool NonInteractive { get; set; } = false;
    public ProviderType SecretProvider { get; set; } = ProviderType.Password;

    public bool DisableSecrets { get; set; } = false;
}
