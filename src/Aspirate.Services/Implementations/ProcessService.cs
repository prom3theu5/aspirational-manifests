using Aspirate.Shared.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Aspirate.Services.Implementations;
public class ProcessService(IAnsiConsole logger) : IProcessService
{
    public ProcessWrapper StartProcess(ProcessStartInfo startInfo)
    {
        var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start process.");
        return new ProcessWrapper(process.Id, process.MainModule?.FileName ?? "Unknown");
    }

    public ProcessWrapper GetProcessById(int processId)
    {
        var process = Process.GetProcessById(processId) ?? throw new InvalidOperationException("Could not get process");
        return new ProcessWrapper(process.Id, process.MainModule?.FileName ?? "Unknown");
    }

    public async Task<bool> KillProcess(int processId)
    {
        bool killedAll = true;
        if (OperatingSystem.IsWindows() && IsChocolateyProcess(processId))
        {
            killedAll = await KillChildProcessesByParentId(processId);
        }

        try
        {
            var process = Process.GetProcessById(processId);

            process.Kill();

            var cts = new CancellationTokenSource();
            cts.CancelAfter(10000);

            await process.WaitForExitAsync(cts.Token);

            if (!process.HasExited)
            {
                killedAll = false;
            }
        }
        catch (Exception ex)
        {
            logger.WriteException(ex);
            killedAll = false;
        }
        return killedAll;
    }

    [SupportedOSPlatform("windows")]
    public async Task<bool> KillChildProcessesByParentId(int parentId)
    {
        bool killedAll = true;

        using (var searcher = new ManagementObjectSearcher("SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = " + parentId))
        {
            foreach (var obj in searcher.Get())
            {
                try
                {
                    var childProcessId = Convert.ToInt32(obj["ProcessId"]);
                    var childProcess = Process.GetProcessById(childProcessId);

                    childProcess.Kill();

                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(10000);

                    await childProcess.WaitForExitAsync(cts.Token);

                    if (!childProcess.HasExited)
                    {
                        killedAll = false;
                    }
                }
                catch (Exception ex)
                {
                    logger.WriteLine(ex.Message);
                    logger.WriteLine($"Could not end child process of process Id: {parentId}");
                    killedAll = false;
                }
            }
        }
        return killedAll;
    }

    public string GetProcessPath(int processId)
    {
        try
        {
            var process = Process.GetProcessById(processId);
            return process.MainModule?.FileName ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    public virtual bool IsChocolateyProcess(int processId)
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        try
        {
            string processPath = GetProcessPath(processId);
            return processPath.Contains("chocolatey");
        }
        catch (Exception ex)
        {
            logger.WriteLine($"Error getting minikube process path: {ex.Message}");
        }
        return false;
    }
}
