namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateKustomizeManifestsAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Kustomize Manifests[/]");

        var outputFormat = OutputFormat.FromValue(CurrentState.OutputFormat);

        if (outputFormat == OutputFormat.DockerCompose)
        {
            Logger.MarkupLine($"[red](!)[/] The output format '{CurrentState.OutputFormat}' is not supported for this action.");
            Logger.MarkupLine("[red](!)[/] Please use the output format 'compose' instead.");
            ActionCausesExitException.ExitNow();
        }

        if (NoSupportedComponentsExitAction())
        {
            return true;
        }

        Logger.MarkupLine("[bold]Generating kustomize manifests to run against your kubernetes cluster:[/]");

        foreach (var resource in CurrentState.AllSelectedSupportedComponents)
        {
            await ProcessIndividualResourceManifests(resource);
        }

        return true;
    }

    private bool NoSupportedComponentsExitAction()
    {
        if (CurrentState.HasSelectedSupportedComponents)
        {
            return false;
        }

        Logger.MarkupLine("[bold]No supported components selected. Skipping generation of kustomize manifests.[/]");
        return true;
    }

    private async Task ProcessIndividualResourceManifests(KeyValuePair<string, Resource> resource)
    {
        var handler = Services.GetKeyedService<IResourceProcessor>(resource.Value.Type);

        if (handler is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unsupported.[/]");
            return;
        }

        var success = await handler.CreateManifests(new()
        {
            Resource = resource,
            OutputPath =  CurrentState.OutputPath,
            ImagePullPolicy =  CurrentState.ImagePullPolicy,
            TemplatePath = CurrentState.TemplatePath,
            DisableSecrets =  CurrentState.DisableSecrets,
            WithPrivateRegistry = CurrentState.WithPrivateRegistry,
            WithDashboard = CurrentState.IncludeDashboard
        });

        if (success && !AspirateState.IsNotDeployable(resource.Value) && resource.Value is not DaprComponentResource)
        {
            CurrentState.AppendToFinalResources(resource.Key, resource.Value);
        }
    }
}
