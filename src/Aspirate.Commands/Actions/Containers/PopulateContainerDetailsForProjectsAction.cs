namespace Aspirate.Commands.Actions.Containers;

public sealed class PopulateContainerDetailsForProjectsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        if (NoSelectedProjectComponents())
        {
            return true;
        }

        await HandleProjects();

        Logger.MarkupLine("\r\n[bold]Gathering Tasks Completed - Cache Populated.[/]");

        return true;
    }

    private async Task HandleProjects()
    {
        var projectProcessor = Services.GetRequiredKeyedService<IProcessor>(AspireComponentLiterals.Project) as ProjectProcessor;

        Logger.MarkupLine("\r\n[bold]Gathering container details for each project in selected components[/]\r\n");

        foreach (var resource in CurrentState.SelectedProjectComponents)
        {
            await projectProcessor.PopulateContainerDetailsCacheForProject(resource, CurrentState.ContainerRegistry, CurrentState.ContainerImageTag);
        }
    }

    private bool NoSelectedProjectComponents()
    {
        if (CurrentState.SelectedProjectComponents.Count != 0)
        {
            return false;
        }

        Logger.MarkupLine("\r\n[bold]No project components selected. Skipping execution of container detail gathering for them.[/]");
        return true;
    }
}
