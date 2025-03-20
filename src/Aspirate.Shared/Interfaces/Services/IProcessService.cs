namespace Aspirate.Shared.Interfaces.Services;
public interface IProcessService
{
    ProcessWrapper StartProcess(ProcessStartInfo startInfo);
    Task<bool> KillProcess(int processId);
    string GetProcessPath(int processId);
    ProcessWrapper GetProcessById(int processId);
    bool IsChocolateyProcess(int processId);
}
