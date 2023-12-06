namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateFinalKustomizeManifestAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (CurrentState.SkipFinalKustomizeGeneration)
        {
            return Task.FromResult(true);
        }

        if (NoSupportedComponentsExitAction())
        {
            return Task.FromResult(true);
        }

        if (!CurrentState.NonInteractive)
        {
            Logger.WriteLine();
            var shouldGenerateFinalKustomizeManifest = Logger.Confirm(
                "[bold]Would you like to generate the top level kustomize manifest to run against your kubernetes cluster?[/]");

            if (!shouldGenerateFinalKustomizeManifest)
            {
                Logger.MarkupLine("[yellow](!)[/] Skipping final manifest");
                return Task.FromResult(true);
            }
        }

        var finalHandler = Services.GetRequiredKeyedService<IProcessor>(AspireLiterals.Final) as FinalProcessor;
        finalHandler.CreateFinalManifest(CurrentState.FinalResources, CurrentState.OutputPath, CurrentState.TemplatePath);

        return Task.FromResult(true);
    }

    private bool NoSupportedComponentsExitAction()
    {
        if (CurrentState.HasSelectedSupportedComponents)
        {
            return false;
        }

        Logger.MarkupLine("\r\n[bold]No supported components selected. Final manifest does not need to be generated as it would be empty.[/]");
        return true;
    }
}
