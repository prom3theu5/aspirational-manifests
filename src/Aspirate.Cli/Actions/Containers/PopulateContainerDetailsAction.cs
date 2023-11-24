namespace Aspirate.Cli.Actions.Containers;

public sealed class PopulateContainerDetailsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "PopulateContainerDetailsAction";

    public override async Task<bool> ExecuteAsync()
    {
        var projectProcessor = Services.GetRequiredKeyedService<IProcessor>(AspireLiterals.Project) as ProjectProcessor;

        Logger.MarkupLine("\r\n[bold]Gathering container details for each project in selected components[/]\r\n");

        foreach (var resource in CurrentState.ComputedParameters.SelectedProjectComponents)
        {
            await projectProcessor.PopulateContainerDetailsCacheForProject(resource, CurrentState.InputParameters.LoadedAspirateSettings);
        }

        Logger.MarkupLine("\r\n[bold]Gathering Tasks Completed - Cache Populated.[/]");

        return true;
    }
}
