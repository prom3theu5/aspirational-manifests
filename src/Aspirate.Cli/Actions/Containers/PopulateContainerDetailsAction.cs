namespace Aspirate.Cli.Actions.Containers;

public sealed class PopulateContainerDetailsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "PopulateContainerDetailsAction";

    public override async Task<bool> ExecuteAsync()
    {
        if (NoSelectedProjectComponents())
        {
            return true;
        }

        var projectProcessor = Services.GetRequiredKeyedService<IProcessor>(AspireLiterals.Project) as ProjectProcessor;

        Logger.MarkupLine("\r\n[bold]Gathering container details for each project in selected components[/]\r\n");

        foreach (var resource in CurrentState.SelectedProjectComponents)
        {
            await projectProcessor.PopulateContainerDetailsCacheForProject(resource, CurrentState.ContainerRegistry, CurrentState.ContainerImageTag);
        }

        Logger.MarkupLine("\r\n[bold]Gathering Tasks Completed - Cache Populated.[/]");

        return true;
    }

    private bool NoSelectedProjectComponents()
    {
        if (CurrentState.SelectedProjectComponents.Count != 0)
        {
            return false;
        }

        Logger.MarkupLine("\r\n[bold]No project components selected. Skipping execution of container detail gathering.[/]");
        return true;

    }
}
