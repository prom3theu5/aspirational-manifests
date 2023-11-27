using Aspirate.Cli.Processors.Project;

namespace Aspirate.Cli.Actions.Containers;

public sealed class BuildAndPushContainersAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "BuildAndPushContainersAction";

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

        var projectProcessor = Services.GetRequiredKeyedService<IProcessor>(AspireLiterals.Project) as ProjectProcessor;

        Logger.MarkupLine("\r\n[bold]Building all project resources, and pushing containers:[/]\r\n");

        foreach (var resource in CurrentState.SelectedProjectComponents)
        {
            await projectProcessor.BuildAndPushProjectContainer(resource, CurrentState.NonInteractive);
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
