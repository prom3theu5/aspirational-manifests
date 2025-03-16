namespace Aspirate.Shared.Interfaces.Services;

public interface IMinikubeCliService
{
    bool IsMinikubeCliInstalledOnMachine();
    Task ActivateMinikubeMount(AspirateState state);
    void KillMinikubeMounts(AspirateState state);
}
