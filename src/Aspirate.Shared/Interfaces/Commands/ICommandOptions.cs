namespace Aspirate.Shared.Interfaces.Commands;

public interface ICommandOptions
{
    bool? NonInteractive { get; set; }
    bool? DisableSecrets { get; set; }
    bool? DisableState { get; set; }
    string? SecretPassword { get; set; }
}
