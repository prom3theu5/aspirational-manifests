namespace Aspirate.Shared.Interfaces.Services;
public interface IProcessService
{
    ProcessWrapper? StartProcess(ProcessStartInfo startInfo);
    ProcessWrapper? GetProcessById(int processId);
    Task<bool> KillProcess(int processId);
    bool IsChocolateyProcess(int processId);
    string GetProcessPath(int processId);
}
