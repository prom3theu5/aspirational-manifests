namespace Aspirate.Shared.Interfaces.Services;

public interface IMinikubeCliService
{
    bool IsMinikubeCliInstalledOnMachine();
    Task ActivateMinikubeMount(AspirateState state);
    Task<bool> KillMinikubeMounts(AspirateState state);
}
