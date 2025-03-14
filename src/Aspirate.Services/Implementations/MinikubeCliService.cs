using System.Diagnostics;
using Aspirate.Shared.Models.AspireManifests;
using Aspirate.Shared.Models.AspireManifests.Components.Common.Container;
using Aspirate.Shared.Models.AspireManifests.Components.V0.Container;
using Aspirate.Shared.Models.AspireManifests.Interfaces;
using Microsoft.Extensions.Logging;
using static IdentityModel.ClaimComparer;

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

            int processCount = 0;
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

                logger.MarkupLine($"[bold]Opening minikube mount at: {bindMount.Source}:/mnt{bindMount.Target}[/]");

                // Start the process without waiting for it to finish
                Process.Start(startInfo);
                processCount++;

                //var argumentsBuilder = ArgumentsBuilder
                //.Create()
                //.AppendArgument("mount", string.Empty, quoteValue: false)
                //.AppendArgument($"{bindMount.Source}:{bindMount.Target}", string.Empty, quoteValue: false)
                //.AppendArgument("--background", string.Empty, quoteValue: false);

                //var result = await shellExecutionService.ExecuteCommand(new()
                //{
                //    Command = _minikubePath,
                //    ArgumentsBuilder = argumentsBuilder,
                //    ShowOutput = true
                //});

                //results.Add(result);
            }
        }

        //return results;
    }

    public async Task<ShellCommandResult> KillMinikubeMounts()
    {
        var argumentsBuilder = ArgumentsBuilder
        .Create()
        .AppendArgument("mount", string.Empty, quoteValue: false)
        .AppendArgument("ssh", string.Empty, quoteValue: false)
        .AppendArgument("pkill", string.Empty, quoteValue: false)
        .AppendArgument("-f", string.Empty, quoteValue: false)
        .AppendArgument("9pnet_virtio", string.Empty, quoteValue: true);

        return await shellExecutionService.ExecuteCommand(new()
        {
            Command = _minikubePath,
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = true
        });
    }
}
