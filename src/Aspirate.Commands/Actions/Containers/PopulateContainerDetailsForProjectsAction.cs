using Aspirate.Processors.Resources.Project;

namespace Aspirate.Commands.Actions.Containers;

public sealed class PopulateContainerDetailsForProjectsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Gathering Information about deployables[/]");

        if (NoSelectedProjectComponents())
        {
            Logger.MarkupLine("[bold]No project components selected. Skipping execution of container detail gathering for them.[/]");
            return true;
        }

        await HandleProjects();

        Logger.MarkupLine("[bold]Gathering Tasks Completed - Cache Populated.[/]");

        return true;
    }

    private async Task HandleProjects()
    {
        var projectProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(AspireComponentLiterals.Project) as ProjectProcessor;

        Logger.MarkupLine("[bold]Gathering container details for each project in selected components[/]");

        foreach (var resource in CurrentState.SelectedProjectComponents)
        {
            await projectProcessor.PopulateContainerDetailsCacheForProject(resource, new()
            {
                Registry = CurrentState.ContainerRegistry,
                Prefix = CurrentState.ContainerRepositoryPrefix,
                Tags = CurrentState.ContainerImageTags,
            });
        }
    }

    private bool NoSelectedProjectComponents()
    {
        if (CurrentState.SelectedProjectComponents.Count != 0)
        {
            return false;
        }

        Logger.MarkupLine("[bold]No project components selected. Skipping execution of container detail gathering for them.[/]");
        return true;
    }
}
