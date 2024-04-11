using Aspirate.Processors.Resources.Dockerfile;
using Aspirate.Services.Parameters;

namespace Aspirate.Commands.Actions.Containers;

public sealed class BuildAndPushContainersFromDockerfilesAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Dockerfiles[/]");

        if (NoSelectedDockerfileComponents())
        {
            return true;
        }

        var dockerfileProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(AspireComponentLiterals.Dockerfile) as DockerfileProcessor;

        CacheContainerDetails(dockerfileProcessor);

        if (CurrentState.SkipBuild)
        {
            var rule = new Rule("[blue][bold]Skipping build and push action as requested.[/][/]");
            AnsiConsole.Write(rule);
            return true;
        }

        await PerformBuildAndPushes(dockerfileProcessor);

        return true;
    }

    private void CacheContainerDetails(DockerfileProcessor? dockerfileProcessor)
    {
        Logger.MarkupLine("[bold]Building all dockerfile resources, and pushing containers[/]");

        foreach (var resource in CurrentState.SelectedDockerfileComponents)
        {
            dockerfileProcessor.PopulateContainerImageCacheWithImage(resource, new()
            {
                Registry = CurrentState.ContainerRegistry,
                Prefix = CurrentState.ContainerRepositoryPrefix,
                Tag = CurrentState.ContainerImageTag,
            });
        }

        Logger.MarkupLine("[bold]Building and push completed for all selected dockerfile components.[/]");
    }

    private async Task PerformBuildAndPushes(DockerfileProcessor? dockerfileProcessor)
    {
        Logger.MarkupLine("[bold]Building all dockerfile resources, and pushing containers:[/]");

        foreach (var resource in CurrentState.SelectedDockerfileComponents)
        {
            await dockerfileProcessor.BuildAndPushContainerForDockerfile(resource, new()
            {
                ContainerBuilder = CurrentState.ContainerBuilder,
                ImageName = resource.Key,
                Registry = CurrentState.ContainerRegistry,
                Prefix = CurrentState.ContainerRepositoryPrefix,
                Tag = CurrentState.ContainerImageTag
            }, CurrentState.NonInteractive);
        }

        Logger.MarkupLine("[bold]Building and push completed for all selected dockerfile components.[/]");
    }

    private bool NoSelectedDockerfileComponents()
    {
        if (CurrentState.SelectedDockerfileComponents.Count != 0)
        {
            return false;
        }

        Logger.MarkupLine("[bold]No Dockerfile components selected. Skipping build and publish action.[/]");
        return true;
    }
}
