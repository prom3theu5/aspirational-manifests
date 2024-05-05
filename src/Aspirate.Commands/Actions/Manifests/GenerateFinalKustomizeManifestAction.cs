namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateFinalKustomizeManifestAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Final Manifest[/]");

        if (CurrentState.SkipFinalKustomizeGeneration == true)
        {
            Logger.MarkupLine("[blue]Skipping final manifest generation as requested.[/]");
            return Task.FromResult(true);
        }

        if (NoSupportedComponentsExitAction())
        {
            return Task.FromResult(true);
        }

        if (!CurrentState.NonInteractive)
        {
            if (!ShouldCreateFinalManifest())
            {
                return Task.FromResult(true);
            }
        }

        var finalHandler = Services.GetRequiredKeyedService<IResourceProcessor>(AspireLiterals.Final) as FinalProcessor;

        finalHandler.CreateFinalManifest(
            CurrentState.FinalResources,
            CurrentState.OutputPath,
            CurrentState.TemplatePath,
            CurrentState.Namespace,
            CurrentState.WithPrivateRegistry,
            CurrentState.PrivateRegistryUrl,
            CurrentState.PrivateRegistryUsername,
            CurrentState.PrivateRegistryPassword,
            CurrentState.PrivateRegistryEmail,
            CurrentState.IncludeDashboard);

        return Task.FromResult(true);
    }

    private bool ShouldCreateFinalManifest()
    {
        if (CurrentState.SkipFinalKustomizeGeneration == false)
        {
            return true;
        }

        var shouldGenerateFinalKustomizeManifest = Logger.Confirm(
            "[bold]Would you like to generate the top level kustomize manifest to run against your kubernetes cluster?[/]");

        CurrentState.SkipFinalKustomizeGeneration = !shouldGenerateFinalKustomizeManifest;

        if (!shouldGenerateFinalKustomizeManifest)
        {
            Logger.MarkupLine("[yellow](!)[/] Skipping final manifest");
            return false;
        }

        return true;
    }

    private bool NoSupportedComponentsExitAction()
    {
        if (CurrentState.HasSelectedSupportedComponents)
        {
            return false;
        }

        Logger.MarkupLine("[bold]No supported components selected. Final manifest does not need to be generated as it would be empty.[/]");
        return true;
    }
}
