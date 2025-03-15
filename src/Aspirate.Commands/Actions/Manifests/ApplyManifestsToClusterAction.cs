namespace Aspirate.Commands.Actions.Manifests;

public sealed class ApplyManifestsToClusterAction(
    IKubernetesService kubernetesClientService,
    IKubeCtlService kubeCtlService,
    ISecretProvider secretProvider,
    IFileSystem fileSystem,
    IDaprCliService daprCliService,
    IKustomizeService kustomizeService,
    IMinikubeCliService minikubeCliService,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Deployment to Cluster[/]");

        var secretFiles = new List<string>();

        try
        {
            await kubernetesClientService.InteractivelySelectKubernetesCluster(CurrentState);

            await HandleDapr();

            await Task.Run(HandleMinikubeMounts);

            await kustomizeService.WriteSecretsOutToTempFiles(CurrentState, secretFiles, secretProvider);
            await kubeCtlService.ApplyManifests(CurrentState.KubeContext, CurrentState.InputPath);
            await HandleRollingRestart();
            Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments successfully applied to cluster [blue]'{CurrentState.KubeContext}'[/]");

            return true;
        }
        catch (Exception e)
        {
            Logger.MarkupLine("[red](!)[/] Failed to apply manifests to cluster.");
            Logger.MarkupLine($"[red](!)[/] Error: {e.Message}");
            return false;
        }
        finally
        {
            kustomizeService.CleanupSecretEnvFiles(CurrentState.DisableSecrets, secretFiles);
        }
    }

    private void HandleMinikubeMounts()
    {
        if (CurrentState.KubeContext != "minikube")
        {
            return;
        }

        Logger.MarkupLine("Applying volume mounts to minikube...");

        var minikubeCliInstalled = minikubeCliService.IsMinikubeCliInstalledOnMachine();

        if (!minikubeCliInstalled)
        {
            Logger.MarkupLine("[yellow]Minikube cli is required to perform Minikube volume mounts.[/]");
            Logger.MarkupLine("[yellow]Please install minikube cli following the guide here:[blue]https://minikube.sigs.k8s.io/docs/start/?arch=%2Fwindows%2Fx86-64%2Fstable%2F.exe+download/[/][/]");
            Logger.MarkupLine("[yellow]Manifest deployment will continue, but Minikube volume mounts will not be applied by aspirate.[/]");
            return;
        }

        minikubeCliService.ActivateMinikubeMount(CurrentState);
        //minikubeCliService.KillMinikubeMounts();

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
            Logger.MarkupLine("[yellow]Dapr cli is required to perform dapr installation in your cluster.[/]");
            Logger.MarkupLine("[yellow]Please install dapr cli following the guide here:[blue]https://docs.dapr.io/getting-started/install-dapr-cli/[/][/]");
            Logger.MarkupLine("[yellow]Manifest deployment will continue, but dapr will not be installed by aspirate.[/]");
            return;
        }

        var daprInstalled = await daprCliService.IsDaprInstalledInCluster();

        if (!daprInstalled)
        {
            Logger.MarkupLine("Dapr is required for this workload as you have dapr components, but is not installed in the cluster.");
            Logger.MarkupLine($"Installing Dapr in cluster [blue]'{CurrentState.KubeContext}'[/]");
            var result = await daprCliService.InstallDaprInCluster();

            if (result.ExitCode != 0)
            {
                Logger.MarkupLine($"[red](!)[/] Failed to install Dapr in cluster [blue]'{CurrentState.KubeContext}'[/]");
                Logger.MarkupLine($"[red](!)[/] Error: {result.Error}");
                ActionCausesExitException.ExitNow();
            }

            Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Dapr installed in cluster [blue]'{CurrentState.KubeContext}'[/]");
        }
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            Logger.ValidationFailed("Cannot apply manifests to cluster without specifying the kubernetes context to use.");
        }

        if (string.IsNullOrEmpty(CurrentState.InputPath))
        {
            Logger.ValidationFailed("Cannot apply manifests to cluster without specifying the input path to use for manifests.");
        }
    }

    private async Task HandleRollingRestart()
    {
        if (CurrentState.RollingRestart != true)
        {
            return;
        }

        var result = await kubeCtlService.PerformRollingRestart(CurrentState.KubeContext, CurrentState.InputPath);

        if (!result)
        {
            Logger.MarkupLine("[red](!)[/] Selected deployment options have failed.");
            ActionCausesExitException.ExitNow();
        }
    }
}
