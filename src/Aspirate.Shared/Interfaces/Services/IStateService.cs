namespace Aspirate.Shared.Interfaces.Services;

public interface IStateService
{
    Task SaveState(StateManagementOptions options);

    Task RestoreState(StateManagementOptions options);
}
