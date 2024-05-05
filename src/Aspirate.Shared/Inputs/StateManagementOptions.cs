namespace Aspirate.Shared.Inputs;

public class StateManagementOptions
{
    public required AspirateState State { get; set; }
    public required bool? DisableState { get; set; }
    public required bool? NonInteractive { get; set; } = false;
}
