namespace Aspirate.Commands.Actions.Containers;

public sealed class BuildAndPushContainersFromDockerfilesAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        if (CurrentState.SkipBuild)
        {
            Logger.MarkupLine("\r\n[bold]Skipping build and push action as requested.[/]");
            return true;
        }

        if (NoSelectedDockerfileComponents())
        {
            return true;
        }

        var dockerfileProcessor = Services.GetRequiredKeyedService<IProcessor>(AspireLiterals.Dockerfile) as DockerfileProcessor;

        Logger.MarkupLine("\r\n[bold]Building all dockerfile resources, and pushing containers:[/]\r\n");

        foreach (var resource in CurrentState.SelectedDockerfileComponents)
        {
            await dockerfileProcessor.BuildAndPushContainerForDockerfile(resource, CurrentState.ContainerBuilder, resource.Key, CurrentState.ContainerRegistry, CurrentState.NonInteractive);
        }

        Logger.MarkupLine("\r\n[bold]Building and push completed for all selected dockerfile components.[/]");

        return true;
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
