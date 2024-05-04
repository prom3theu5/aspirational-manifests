namespace Aspirate.Shared.Interfaces.Services;

public interface IStateService
{
    Task SaveState(AspirateState state);

    Task RestoreState(AspirateState state);
}
