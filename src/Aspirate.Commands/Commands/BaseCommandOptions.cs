namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptions : ICommandOptions
{
    public bool? NonInteractive { get; set; }
    public bool? DisableSecrets { get; set; }
    public bool? DisableState { get; set; }
    public string? SecretPassword { get; set; }
    public string? LaunchProfile { get; set; }
    public virtual bool RequiresSecrets => true;
}
