namespace Aspirate.Commands.Actions.Manifests;

public sealed class RemoveManifestsFromClusterAction(
    IKubeCtlService kubeCtlService,
    IServiceProvider serviceProvider,
    IFileSystem fileSystem,
    ISecretProvider secretProvider) :
    BaseActionWithNonInteractiveValidation(serviceProvider)
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
                    "[bold]Would you like to remove the deployed manifests from a kubernetes cluster defined in your kubeconfig file?[/]");

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

            CreateEmptySecretFiles(secretFiles);
            await kubeCtlService.RemoveManifests(CurrentState.KubeContext, CurrentState.InputPath);
            Logger.MarkupLine(
                $"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments removed from cluster [blue]'{CurrentState.KubeContext}'[/]");

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

    private void CreateEmptySecretFiles(List<string> files)
    {
        if (secretProvider is PasswordSecretProvider passwordSecretProvider)
        {
            passwordSecretProvider.LoadState(CurrentState.InputPath);

            foreach (var resourceSecrets in passwordSecretProvider.State.Secrets.Where(x=>x.Value.Keys.Count > 0))
            {
                var secretFile = fileSystem.Path.Combine(CurrentState.InputPath, resourceSecrets.Key, $".{resourceSecrets.Key}.secrets");

                files.Add(secretFile);

                var stream = fileSystem.File.Create(secretFile);
                stream.Close();
            }
        }
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            NonInteractiveValidationFailed("Cannot remove manifests from a cluster without specifying the kubernetes context to use.");
        }

        if (string.IsNullOrEmpty(CurrentState.InputPath))
        {
            NonInteractiveValidationFailed("Cannot remove manifests from a cluster without specifying the input path to use for manifests.");
        }
    }

    private void CleanupSecretEnvFiles(IEnumerable<string> secretFiles)
    {
        foreach (var secretFile in secretFiles.Where(secretFile => fileSystem.File.Exists(secretFile)))
        {
            fileSystem.File.Delete(secretFile);
        }
    }
}
