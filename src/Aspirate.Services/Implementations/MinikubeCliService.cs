using System.Reflection.Metadata.Ecma335;

namespace Aspirate.Services.Implementations;

public class MinikubeCliService(IShellExecutionService shellExecutionService, IAnsiConsole logger, IServiceProvider serviceProvider) : IMinikubeCliService
{
    private string _minikubePath = "minikube";

    private const string DefaultMountPath = "/mount";
    private const string MountCommand = "mount";

    public bool IsMinikubeCliInstalledOnMachine()
    {
        var result = shellExecutionService.IsCommandAvailable("minikube");

        if (!result.IsAvailable)
        {
            return false;
        }

        _minikubePath = result.FullPath;
        return true;
    }

    public void ActivateMinikubeMount(AspirateState state)
    {
        int count = 0;
        foreach (var resourceWithMounts in state.BindMounts)
        {
            var source = resourceWithMounts.Key;
            var targets = resourceWithMounts.Value;

            foreach (var target in targets.Keys)
            {
                if (string.IsNullOrWhiteSpace(target))
                {
                    logger.WriteLine("Mount target was null or empty - skipping this mount.");
                    continue;
                }

                string args = string.Concat(MountCommand, " ", source, ":", DefaultMountPath, target);

                var startInfo = new ProcessStartInfo
                {
                    FileName = _minikubePath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                logger.MarkupLine($"[cyan]Executing: {_minikubePath} {args}[/]");

                var process = Process.Start(startInfo);

                if (IsChocolateyProcess(process) && count == 0)
                {
                    logger.MarkupLine($"[blue]minikube runs through Chocolatey shim. Process path: {process.MainModule.FileName}[/]");
                }

                targets[target] = process.Id;
            }
            count++;
        }
        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Started minikube mount processes [blue][/]");
    }

    public bool IsChocolateyProcess(Process process)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        try
        {
            string processPath = process.MainModule?.FileName ?? "Unknown";

            if (processPath.Contains("chocolatey"))
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.WriteLine($"Error getting minikube process path: {ex.Message}");
        }
        return false;
    }

    public void KillMinikubeMounts(AspirateState state)
    {
        foreach (var resourceWithMounts in state.BindMounts)
        {
            var resource = resourceWithMounts.Key;
            var bindMounts = resourceWithMounts.Value;

            var processIds = bindMounts.Values.Where(processId => processId > 0);

            foreach (var processId in processIds)
            {
                var process = Process.GetProcessById(processId);

                if (IsChocolateyProcess(process))
                {
#pragma warning disable CA1416
                    using (var searcher = new ManagementObjectSearcher("SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = " + processId))
                    {
                        foreach (var obj in searcher.Get())
                        {
                            try
                            {
                                var childProcessId = Convert.ToInt32(obj["ProcessId"]);
                                var childProcess = Process.GetProcessById(childProcessId);

                                childProcess?.Kill();
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLine(ex.Message);
                                logger.WriteLine($"Could not end child process of process Id: {processId}");
                            }
                        }
                    }
#pragma warning restore CA1416
                }
                process.Kill();
            }
        }
    }
}
