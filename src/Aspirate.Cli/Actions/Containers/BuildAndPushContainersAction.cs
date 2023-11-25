namespace Aspirate.Cli.Actions.Containers;

public sealed class BuildAndPushContainersAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "BuildAndPushContainersAction";

    public override async Task<bool> ExecuteAsync()
    {
        if (NoSelectedProjectComponents())
        {
            return true;
        }

        var projectProcessor = Services.GetRequiredKeyedService<IProcessor>(AspireLiterals.Project) as ProjectProcessor;

        Logger.MarkupLine("\r\n[bold]Building all project resources, and pushing containers:[/]\r\n");

        foreach (var resource in CurrentState.ComputedParameters.SelectedProjectComponents)
        {
            await projectProcessor.BuildAndPushProjectContainer(resource);
        }

        Logger.MarkupLine("\r\n[bold]Building and push completed for all selected project components.[/]");

        return true;
    }

    private bool NoSelectedProjectComponents()
    {
        if (CurrentState.ComputedParameters.SelectedProjectComponents.Count != 0)
        {
            return false;
        }

        Logger.MarkupLine("\r\n[bold]No project components selected. Skipping build and publish action.[/]");
        return true;
    }
}
