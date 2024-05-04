namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptions : ICommandOptions
{
    public bool NonInteractive { get; set; }
    public SecretProviderType SecretProvider { get; set; }
    public bool DisableSecrets { get; set; }
    public bool DisableState { get; set; }
    public string? SecretPassword { get; set; }
}
