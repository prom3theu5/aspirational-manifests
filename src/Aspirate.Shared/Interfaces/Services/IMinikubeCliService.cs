namespace Aspirate.Shared.Interfaces.Services;

public interface IMinikubeCliService
{
    bool IsMinikubeCliInstalledOnMachine();
    void ActivateMinikubeMount(AspirateState state);
    void KillMinikubeMounts(AspirateState state);
}
