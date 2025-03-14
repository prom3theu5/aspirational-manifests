namespace Aspirate.Shared.Interfaces.Services;

public interface IMinikubeCliService
{
    bool IsMinikubeCliInstalledOnMachine();
    void ActivateMinikubeMount(AspirateState state);
    Task<ShellCommandResult> KillMinikubeMounts();
}
