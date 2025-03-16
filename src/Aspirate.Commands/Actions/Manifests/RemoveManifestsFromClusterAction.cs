namespace Aspirate.Commands.Actions.Manifests;

public sealed class RemoveManifestsFromClusterAction(
    IKubeCtlService kubeCtlService,
    IServiceProvider serviceProvider,
    IFileSystem fileSystem,
    IDaprCliService daprCliService,
    IMinikubeCliService minikubeCliService,
    ISecretProvider secretProvider) :
    BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Removal from Cluster[/]");

        var secretFiles = new List<string>();

        try
        {
            await InteractivelySelectKubernetesCluster();

            CreateEmptySecretFiles(secretFiles);
            await kubeCtlService.RemoveManifests(CurrentState.KubeContext, CurrentState.InputPath);
            Logger.MarkupLine(
                $"[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments removed from cluster [blue]'{CurrentState.KubeContext}'[/]");

            await HandleDapr();

            return true;
        }
        catch (Exception e)
        {
            Logger.MarkupLine("[red](!)[/] Failed to remove manifests from cluster.");
            Logger.MarkupLine($"[red](!)[/] Error: {e.Message}");
            return false;
        }
        finally
        {
            CleanupSecretEnvFiles(secretFiles);
        }
    }

    private async Task InteractivelySelectKubernetesCluster()
    {
        if (CurrentState.ActiveKubernetesContextIsSet)
        {
            return;
        }

        var shouldDeploy = Logger.Confirm(
            "[bold]Would you like to remove the deployed manifests from a kubernetes cluster defined in your kubeconfig file?[/]");

        if (!shouldDeploy)
        {
            Logger.MarkupLine("[yellow]Skipping deployment of manifests to cluster.[/]");
            ActionCausesExitException.ExitNow();
        }

        CurrentState.KubeContext = await kubeCtlService.SelectKubernetesContextForDeployment();

        if (string.IsNullOrEmpty(CurrentState.KubeContext))
        {
            Logger.MarkupLine("[red]Failed to set active kubernetes context.[/]");
            ActionCausesExitException.ExitNow();
        }
    }

    private void CreateEmptySecretFiles(List<string> files)
    {
        if (CurrentState.DisableSecrets == true)
        {
            return;
        }

        if (!secretProvider.SecretStateExists(CurrentState))
        {
            return;
        }

        secretProvider.LoadState(CurrentState);

        if (secretProvider.State?.Secrets is null || secretProvider.State.Secrets.Count == 0)
        {
            return;
        }

        foreach (var resourceSecrets in secretProvider.State.Secrets.Where(x => x.Value.Keys.Count > 0))
        {
            var resourcePath = fileSystem.Path.Combine(CurrentState.InputPath, resourceSecrets.Key);

            if (!fileSystem.Directory.Exists(resourcePath))
            {
                continue;
            }

            var secretFile = fileSystem.Path.Combine(resourcePath, $".{resourceSecrets.Key}.secrets");

            files.Add(secretFile);

            var stream = fileSystem.File.Create(secretFile);
            stream.Close();
        }
    }

    private async Task HandleDapr()
    {
        if (!fileSystem.Directory.Exists(fileSystem.Path.Combine(CurrentState.InputPath, "dapr")))
        {
            return;
        }

        var daprCliInstalled = daprCliService.IsDaprCliInstalledOnMachine();

        if (!daprCliInstalled)
        {
            Logger.MarkupLine("[yellow]Dapr cli is required to remove dapr installation from your cluster.[/]");
            Logger.MarkupLine("[yellow]Please install dapr cli following the guide here:[blue]https://docs.dapr.io/getting-started/install-dapr-cli/[/][/]");
            Logger.MarkupLine("[yellow]Manifest removal will continue, but dapr will not be removed by aspirate.[/]");
            return;
        }

        var daprInstalled = await daprCliService.IsDaprInstalledInCluster();

        if (daprInstalled)
        {
            Logger.MarkupLine("Dapr was required for the deployment workloads as you have dapr components.");
            Logger.MarkupLine("Dapr is installed in your cluster.");

            var shouldRemoveDapr = true;

            if (!CurrentState.NonInteractive)
            {
                shouldRemoveDapr = Logger.Confirm("[bold]Would you like to remove Dapr from your cluster?[/]");
            }

            if (shouldRemoveDapr)
            {
                var result = await daprCliService.RemoveDaprFromCluster();

                if (result.ExitCode != 0)
                {
                    Logger.MarkupLine($"[red](!)[/] Failed to remove Dapr from cluster [blue]'{CurrentState.KubeContext}'[/]");
                    Logger.MarkupLine($"[red](!)[/] Error: {result.Error}");
                    ActionCausesExitException.ExitNow();
                }

                Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Dapr removed from cluster [blue]'{CurrentState.KubeContext}'[/]");
            }
        }
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            Logger.ValidationFailed("Cannot remove manifests from a cluster without specifying the kubernetes context to use.");
        }

        if (string.IsNullOrEmpty(CurrentState.InputPath))
        {
            Logger.ValidationFailed("Cannot remove manifests from a cluster without specifying the input path to use for manifests.");
        }
    }

    private void CleanupSecretEnvFiles(IEnumerable<string> secretFiles)
    {
        if (CurrentState.DisableSecrets == true)
        {
            return;
        }

        foreach (var secretFile in secretFiles.Where(secretFile => fileSystem.File.Exists(secretFile)))
        {
            fileSystem.File.Delete(secretFile);
        }
    }
}
