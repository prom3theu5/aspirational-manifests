using System.Diagnostics;
using System.Security.Cryptography;
using Aspirate.Shared.Models.AspireManifests;
using Aspirate.Shared.Models.AspireManifests.Components.Common.Container;
using Aspirate.Shared.Models.AspireManifests.Components.V0.Container;
using Aspirate.Shared.Models.AspireManifests.Interfaces;
using Microsoft.Extensions.Logging;
using System.Management;

namespace Aspirate.Services.Implementations;

public class MinikubeCliService(IShellExecutionService shellExecutionService, IAnsiConsole logger, IServiceProvider serviceProvider) : IMinikubeCliService
{
    private string _minikubePath = "minikube";

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
        foreach (var resourceWithMounts in state.BindMounts)
        {
            var resource = resourceWithMounts.Key;
            var bindMounts = resourceWithMounts.Value;

            foreach (var bindMount in bindMounts)
            {
                if (string.IsNullOrWhiteSpace(bindMount?.Source) || string.IsNullOrWhiteSpace(bindMount?.Target))
                {
                    logger.WriteLine("Mount source or target was null or empty - skipping this mount.");
                    continue;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = "minikube",
                    Arguments = $"mount {bindMount.Source}:/mnt{bindMount.Target}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true // Ensures the process doesn't open a terminal window
                };

                logger.MarkupLine($"[cyan]Opening minikube mount at: {bindMount.Source}:/mnt{bindMount.Target}[/]");

                var process = Process.Start(startInfo);

                if (IsChocolateyProcess(process))
                {
                    logger.MarkupLine($"[blue]minikube runs through Chocolatey shim. Process path: {process.MainModule.FileName}[/]");
                    logger.MarkupLine($"[blue]Will keep track of Chocolatey shim process.[/]");
                }

                bindMount.MinikubeMountProcessId = process.Id;

                logger.MarkupLine($"[blue]minikube mount process Id: {process.Id} - process name: {process.ProcessName}[/]");

                //results.Add(result);
            }
        }

        //return results;
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
            if (processPath.Contains("chocolatey\\bin"))
            {
                return true;
            }
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

            foreach (var bindMount in bindMounts)
            {
                var processId = bindMount.MinikubeMountProcessId ?? 0;

                if (processId > 0)
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

                                    if (childProcess != null)
                                    {
                                        childProcess.Kill();
                                    }
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
}
