namespace Aspirate.Contracts.Models.State;

public class AspirateState
{
    public InputParametersState InputParameters { get; set; } = new();
    public ComputedParametersState ComputedParameters { get; set; } = new();
}
