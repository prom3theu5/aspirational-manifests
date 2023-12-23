namespace Aspirate.Commands.Actions.Manifests;

public sealed class ApplyManifestsToClusterAction(
    IKubeCtlService kubeCtlService,
    ISecretProvider secretProvider,
    IFileSystem fileSystem,
    IDaprCliService daprCliService,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        var secretFiles = new List<string>();

        try
        {
            if (!CurrentState.NonInteractive)
            {
                Logger.WriteLine();
                var shouldDeploy = Logger.Confirm(
                    "[bold]Would you like to deploy the generated manifests to a kubernetes cluster defined in your kubeconfig file?[/]");

                if (!shouldDeploy)
                {
                    Logger.MarkupLine("[yellow]Cancelled![/]");

                    return true;
                }

                CurrentState.KubeContext = await kubeCtlService.SelectKubernetesContextForDeployment();

                if (!CurrentState.ActiveKubernetesContextIsSet)
                {
                    return false;
                }
            }

            await HandleDapr();

            await WriteSecretsOutToTempFiles(secretFiles);
            await kubeCtlService.ApplyManifests(CurrentState.KubeContext, CurrentState.InputPath);
            Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments successfully applied to cluster [blue]'{CurrentState.KubeContext}'[/]");

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
            CleanupSecretEnvFiles(secretFiles);
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
            Logger.MarkupLine("[yellow]Dapr cli is required to perform dapr installation in your cluster.[/]");
            Logger.MarkupLine("[yellow]Please install dapr cli following the guide here:[blue]https://docs.dapr.io/getting-started/install-dapr-cli/[/][/]");
            Logger.MarkupLine("[yellow]Manifest deployment will continue, but dapr will not be installed by aspirate.[/]");
            return;
        }

        var daprInstalled = await daprCliService.IsDaprInstalledInCluster();

        if (!daprInstalled)
        {
            Logger.MarkupLine("Dapr is required for this workload as you have dapr components, but is not installed in the cluster.");
            Logger.MarkupLine($"\r\nInstalling Dapr in cluster [blue]'{CurrentState.KubeContext}'[/]");
            var result = await daprCliService.InstallDaprInCluster();

            if (result.ExitCode != 0)
            {
                Logger.MarkupLine($"[red](!)[/] Failed to install Dapr in cluster [blue]'{CurrentState.KubeContext}'[/]");
                Logger.MarkupLine($"[red](!)[/] Error: {result.Error}");
                throw new ActionCausesExitException(9999);
            }

            Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Dapr installed in cluster [blue]'{CurrentState.KubeContext}'[/]");
        }
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            NonInteractiveValidationFailed("Cannot apply manifests to cluster without specifying the kubernetes context to use.");
        }

        if (string.IsNullOrEmpty(CurrentState.InputPath))
        {
            NonInteractiveValidationFailed("Cannot apply manifests to cluster without specifying the input path to use for manifests.");
        }
    }

    private async Task WriteSecretsOutToTempFiles(List<string> files)
    {
        if (CurrentState.DisableSecrets)
        {
            return;
        }

        if (!secretProvider.SecretStateExists(CurrentState.InputPath))
        {
            return;
        }

        if (secretProvider is PasswordSecretProvider passwordSecretProvider)
        {
            if (passwordSecretProvider.State?.Secrets is null || passwordSecretProvider.State.Secrets.Count == 0)
            {
                return;
            }

            Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Decrypting secrets from [blue]{CurrentState.InputPath}[/]");

            foreach (var resourceSecrets in passwordSecretProvider.State.Secrets.Where(x=>x.Value.Keys.Count > 0))
            {
                var secretFile = fileSystem.Path.Combine(CurrentState.InputPath, resourceSecrets.Key, $".{resourceSecrets.Key}.secrets");

                files.Add(secretFile);

                await using var streamWriter = fileSystem.File.CreateText(secretFile);

                foreach (var key in resourceSecrets.Value.Keys)
                {
                    var secretValue = secretProvider.GetSecret(resourceSecrets.Key, key);
                    await streamWriter.WriteLineAsync($"{key}={secretValue}");
                }

                await streamWriter.FlushAsync();
                streamWriter.Close();
            }
        }
    }

    private void CleanupSecretEnvFiles(IEnumerable<string> secretFiles)
    {
        if (CurrentState.DisableSecrets)
        {
            return;
        }

        foreach (var secretFile in secretFiles.Where(secretFile => fileSystem.File.Exists(secretFile)))
        {
            fileSystem.File.Delete(secretFile);
        }
    }
}
