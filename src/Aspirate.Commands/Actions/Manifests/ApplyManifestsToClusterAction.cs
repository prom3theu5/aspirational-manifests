namespace Aspirate.Commands.Actions.Manifests;

public sealed class ApplyManifestsToClusterAction(
    IKubeCtlService kubeCtlService,
    ISecretProvider secretProvider,
    IFileSystem fileSystem,
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

        if (secretProvider is PasswordSecretProvider passwordSecretProvider)
        {
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
