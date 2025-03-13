using Aspirate.Shared.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Aspirate.Services.Implementations;
public class ProcessService(IAnsiConsole logger) : IProcessService
{
    private const string MinikubeHomeEnvVar = "MINIKUBE_HOME";
    private const string MinikubeFolder = ".minikube";
    private static readonly string _mountProcessFilePath = Path.Combine("profiles", "minikube", ".mount-process");
    public ProcessWrapper? StartProcess(ProcessStartInfo startInfo)
    {
        try
        {
            var process = Process.Start(startInfo);

            if (process == null)
            {
                return null;
            }

            return new ProcessWrapper(process.Id, process.MainModule?.FileName ?? "Unknown");
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task RemoveProcessIdFromMountProcessFile(string processId)
    {
        string path = "";
        string minikubeHome = Environment.GetEnvironmentVariable(MinikubeHomeEnvVar);
        string homeDir = Environment.GetEnvironmentVariable(OperatingSystem.IsWindows() ? "USERPROFILE" : "HOME");

        if (string.IsNullOrWhiteSpace(minikubeHome))
        {
            if (string.IsNullOrWhiteSpace(homeDir))
            {
                logger.MarkupLine($"Could not get home directory. Please manually remove {processId} from {_mountProcessFilePath} within your .minikube folder.");
                return;
            }
            path = Path.Combine(homeDir, MinikubeFolder, _mountProcessFilePath);
        }
        else
        {
            path = Path.Combine(minikubeHome, _mountProcessFilePath);
        }

        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            try
            {
                string content = await File.ReadAllTextAsync(path);

                var processIdsInFile = new List<string>(content.Split(' '));

                processIdsInFile.Remove(processId);

                if (processIdsInFile.Count == 1 && string.IsNullOrWhiteSpace(processIdsInFile.First()))
                {
                    File.Delete(path);
                    return;
                }

                content = string.Join(" ", processIdsInFile);

                await File.WriteAllTextAsync(path, content);
            }
            catch (Exception ex)
            {
                logger.WriteException(ex);
                logger.MarkupLine($"[yellow]Failed at updating .process-mount file. Please manually remove the mount process Ids from the file: {path}[/]");
            }
        }
    }

    public ProcessWrapper? GetProcessById(int processId)
    {
        try
        {
            var process = Process.GetProcessById(processId);
            return new ProcessWrapper(process.Id, process.MainModule?.FileName ?? "Unknown");
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public async Task<bool> KillProcess(int processId)
    {
        bool killedAll = true;
        bool isChocolateyProcess = IsChocolateyProcess(processId);
        bool isWindows = OperatingSystem.IsWindows();

        if (isWindows && isChocolateyProcess)
        {
            return await KillChildProcessesByParentId(processId);
        }
        else
        {
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
                else
                {
                    await RemoveProcessIdFromMountProcessFile(processId.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.WriteException(ex);
                killedAll = false;
            }
            return killedAll;
        }
    }

    [SupportedOSPlatform("windows")]
    public async Task<bool> KillChildProcessesByParentId(int parentId)
    {
        bool killedAll = true;

        using (var searcher = new ManagementObjectSearcher("SELECT ProcessId FROM Win32_Process WHERE Name ='minikube.exe' AND ParentProcessId = " + parentId))
        using (var collection = searcher.Get())
        {
            foreach (var obj in collection)
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
                    else
                    {
                        await RemoveProcessIdFromMountProcessFile(childProcessId.ToString());
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
