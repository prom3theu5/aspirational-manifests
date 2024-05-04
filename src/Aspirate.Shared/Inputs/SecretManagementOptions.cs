namespace Aspirate.Shared.Inputs;

public class SecretManagementOptions
{
    public required bool DisableSecrets { get; set; }
    public required bool NonInteractive { get; set; }
    public required string SecretPassword { get; set; }
    public required AspirateState State { get; set; }
}
