using Aspirate.Processors.Resources.Dockerfile;

namespace Aspirate.Commands.Actions.Containers;

public sealed class BuildAndPushContainersFromDockerfilesAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        if (NoSelectedDockerfileComponents())
        {
            return true;
        }

        var dockerfileProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(AspireComponentLiterals.Dockerfile) as DockerfileProcessor;

        CacheContainerDetails(dockerfileProcessor);

        if (CurrentState.SkipBuild)
        {
            Logger.MarkupLine("\r\n[bold]Skipping build and push action as requested.[/]");
            return true;
        }

        await PerformBuildAndPushes(dockerfileProcessor);

        return true;
    }

    private void CacheContainerDetails(DockerfileProcessor? dockerfileProcessor)
    {
        Logger.MarkupLine("\r\n[bold]Building all dockerfile resources, and pushing containers:[/]\r\n");

        foreach (var resource in CurrentState.SelectedDockerfileComponents)
        {
            dockerfileProcessor.PopulateContainerImageCacheWithImage(resource, resource.Key, CurrentState.ContainerRegistry);
        }

        Logger.MarkupLine("\r\n[bold]Building and push completed for all selected dockerfile components.[/]");
    }

    private async Task PerformBuildAndPushes(DockerfileProcessor? dockerfileProcessor)
    {
        Logger.MarkupLine("\r\n[bold]Building all dockerfile resources, and pushing containers:[/]\r\n");

        foreach (var resource in CurrentState.SelectedDockerfileComponents)
        {
            await dockerfileProcessor.BuildAndPushContainerForDockerfile(resource, CurrentState.ContainerBuilder, resource.Key, CurrentState.ContainerRegistry, CurrentState.NonInteractive);
        }

        Logger.MarkupLine("\r\n[bold]Building and push completed for all selected dockerfile components.[/]");
    }

    private bool NoSelectedDockerfileComponents()
    {
        if (CurrentState.SelectedDockerfileComponents.Count != 0)
        {
            return false;
        }

        Logger.MarkupLine("\r\n[bold]No Dockerfile components selected. Skipping build and publish action.[/]");
        return true;
    }
}
