namespace Aspirate.Services.Interfaces;

public interface IDaprCliService
{
    Task<bool> IsDaprCliInstalledOnMachine();
    Task<bool> IsDaprInstalledInCluster();
    Task<ShellCommandResult> InstallDaprInCluster();
    Task<ShellCommandResult> RemoveDaprFromCluster();
}
