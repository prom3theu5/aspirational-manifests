namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateHelmChartAction(
    IHelmChartCreator helmChartCreator,
    IKustomizeService kustomizeService,
    ISecretProvider secretProvider,
    IFileSystem fileSystem,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Helm Support[/]");

        var outputFormat = OutputFormat.FromValue(CurrentState.OutputFormat);

        if (outputFormat != OutputFormat.Helm)
        {
            Logger.MarkupLine($"[red](!)[/] The output format '{CurrentState.OutputFormat}' is not supported for this action.");
            Logger.MarkupLine("[red](!)[/] Please use the output format 'helm' instead.");
            ActionCausesExitException.ExitNow();
        }

        if (NoSupportedComponentsExitAction())
        {
            return true;
        }

        var kubeObjects = ConvertResourcesToKubeObjects(CurrentState.AllSelectedSupportedComponents);
        await helmChartCreator.CreateHelmChart(kubeObjects, Path.Combine(CurrentState.OutputPath, "Chart"), "AspireProject", CurrentState.IncludeDashboard.GetValueOrDefault());

        return true;
    }

    private List<object> ConvertResourcesToKubeObjects(List<KeyValuePair<string, Resource>> supportedResources)
    {
        var kubernetesObjects = new List<object>();

        foreach (var resource in supportedResources)
        {
            kubernetesObjects.AddRange(ProcessIndividualResourceManifests(resource));
        }

        return kubernetesObjects;
    }

    private List<object> ProcessIndividualResourceManifests(KeyValuePair<string, Resource> resource)
    {
        var handler = Services.GetKeyedService<IResourceProcessor>(resource.Value.Type);

        if (handler is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unsupported.[/]");
            return [];
        }

        return handler.CreateKubernetesObjects(new()
        {
            Resource = resource,
            ImagePullPolicy =  CurrentState.ImagePullPolicy,
            DisableSecrets =  CurrentState.DisableSecrets,
            WithPrivateRegistry = CurrentState.WithPrivateRegistry,
            WithDashboard = CurrentState.IncludeDashboard,
        });
    }

    private bool NoSupportedComponentsExitAction()
    {
        if (CurrentState.HasSelectedSupportedComponents)
        {
            return false;
        }

        Logger.MarkupLine("[bold]No supported components selected. Helm chart will not be created.[/]");
        return true;
    }
}
