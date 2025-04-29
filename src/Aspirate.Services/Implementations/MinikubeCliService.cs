namespace Aspirate.Services.Implementations;

public class MinikubeCliService(IShellExecutionService shellExecutionService, IAnsiConsole logger, IProcessService processService) : IMinikubeCliService
{
    public int DefaultDelay { get; set; } = 60000;

    private string _minikubePath = MinikubeLiterals.Path;

    private const string DefaultMountPath = MinikubeLiterals.HostPathPrefix;
    private const string MountCommand = MinikubeLiterals.MountCommand;

    public bool IsMinikubeCliInstalledOnMachine()
    {
        var result = shellExecutionService.IsCommandAvailable(_minikubePath);

        if (!result.IsAvailable)
        {
            return false;
        }

        _minikubePath = result.FullPath;
        return true;
    }

    public async Task ActivateMinikubeMount(AspirateState state)
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

                var process = processService.StartProcess(startInfo);

                if (process == null)
                {
                    logger.WriteLine("[red]Failed to start minikube mount process[/]");
                    continue;
                }

                if (processService.IsChocolateyProcess(process.Id) && count == 0)
                {
                    logger.MarkupLine($"[blue]minikube runs through Chocolatey shim.[/]");
                }

                targets[target] = process.Id;
            }
            count++;
        }
        logger.MarkupLine($"[yellow]{DefaultDelay / 1000} second wait to let minikube mounts finish setting up[/]");
        await Task.Delay(DefaultDelay);
        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Started minikube mount processes [blue][/]");
    }

    public async Task<bool> KillMinikubeMounts(AspirateState state)
    {
        bool killedAll = true;
        foreach (var resourceWithMounts in state.BindMounts)
        {
            var resource = resourceWithMounts.Key;
            var bindMounts = resourceWithMounts.Value;

            var processIds = bindMounts.Values.Where(processId => processId > 0);

            foreach (var processId in processIds)
            {
                var process = processService.GetProcessById(processId);

                if (process == null)
                {
                    logger.MarkupLine($"[red]Could not get process with Id: {processId} Please verify if it actually exists, and kill it manually.[/]");
                    killedAll = false;
                    continue;
                }

                var result = await processService.KillProcess(processId);
                if (!result)
                {
                    killedAll = false;
                }
            }
        }
        return killedAll;
    }
}
