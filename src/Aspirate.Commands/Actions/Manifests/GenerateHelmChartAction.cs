namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateHelmChartAction(
    IHelmChartCreator helmChartCreator,
    IKubernetesService kubernetesClientService,
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

        var kubeObjects = kubernetesClientService.ConvertResourcesToKubeObjects(CurrentState.AllSelectedSupportedComponents, CurrentState, true);
        await helmChartCreator.CreateHelmChart(kubeObjects, Path.Combine(CurrentState.OutputPath, "Chart"), "AspireProject", CurrentState.IncludeDashboard.GetValueOrDefault());

        return true;
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
