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

        if (CurrentState.SkipHelmGeneration)
        {
            Logger.MarkupLine("[blue]Skipping helm chart generation as requested.[/]");
            return true;
        }

        if (NoSupportedComponentsExitAction())
        {
            return true;
        }

        if (!CurrentState.NonInteractive)
        {
            if (!ShouldCreateHelmChart())
            {
                return true;
            }
        }

        var secretFiles = new List<string>();

        try
        {
            await kustomizeService.WriteSecretsOutToTempFiles(CurrentState, secretFiles, secretProvider);
            await helmChartCreator.CreateHelmChart(CurrentState.OutputPath, Path.Combine(CurrentState.OutputPath, "Chart"), "AspireProject");
        }
        catch (Exception e)
        {
            Logger.MarkupLine("[red](!)[/] Failed to generate helm chart.");
            Logger.MarkupLine($"[red](!)[/] Error: {e.Message}");
            return false;
        }
        finally
        {
            kustomizeService.CleanupSecretEnvFiles(CurrentState.DisableSecrets, secretFiles);
        }

        return true;
    }

    private bool ShouldCreateHelmChart()
    {
        var shouldGenerateHelmChart = Logger.Confirm(
            "[bold]Would you like to generate a helm chart based on the generated kustomize manifests?[/]",
            false);

        if (!shouldGenerateHelmChart)
        {
            Logger.MarkupLine("[blue]Skipping helm chart generation as requested.[/]");
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

        Logger.MarkupLine("[bold]No supported components selected. Helm chart will not be created.[/]");
        return true;
    }
}
