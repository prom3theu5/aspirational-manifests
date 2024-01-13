using Aspirate.Processors.Resources.Project;

namespace Aspirate.Commands.Actions.Containers;

public sealed class BuildAndPushContainersFromProjectsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        if (CurrentState.SkipBuild)
        {
            Logger.MarkupLine("\r\n[bold]Skipping build and push action as requested.[/]");
            return true;
        }

        if (NoSelectedProjectComponents())
        {
            return true;
        }

        var projectProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(AspireComponentLiterals.Project) as ProjectProcessor;

        Logger.MarkupLine("\r\n[bold]Building all project resources, and pushing containers:[/]\r\n");

        foreach (var resource in CurrentState.SelectedProjectComponents)
        {
            await projectProcessor.BuildAndPushProjectContainer(resource, new()
            {
                ContainerBuilder = CurrentState.ContainerBuilder,
                Prefix = CurrentState.ContainerRepositoryPrefix,
            }, CurrentState.NonInteractive);
        }

        Logger.MarkupLine("\r\n[bold]Building and push completed for all selected project components.[/]");

        return true;
    }

    private bool NoSelectedProjectComponents()
    {
        if (CurrentState.SelectedProjectComponents.Count != 0)
        {
            return false;
        }

        Logger.MarkupLine("\r\n[bold]No project components selected. Skipping build and publish action.[/]");
        return true;
    }
}
