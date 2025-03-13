namespace Aspirate.Shared.Interfaces.Services;

public interface IDaprCliService
{
    bool IsDaprCliInstalledOnMachine();
    Task<bool> IsDaprInstalledInCluster();
    Task<ShellCommandResult> InstallDaprInCluster();
    Task<ShellCommandResult> RemoveDaprFromCluster();
}
