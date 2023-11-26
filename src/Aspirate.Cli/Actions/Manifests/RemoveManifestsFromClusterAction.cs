namespace Aspirate.Cli.Actions.Manifests;

public sealed class RemoveManifestsFromClusterAction(IKubeCtlService kubeCtlService, IServiceProvider serviceProvider) :
    BaseActionWithNonInteractiveSupport(serviceProvider)
{
    public const string ActionKey = "RemoveManifestsFromClusterAction";

    public override async Task<bool> ExecuteAsync()
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

        await kubeCtlService.RemoveManifests(CurrentState.KubeContext, CurrentState.InputPath);
        Logger.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments removed from cluster [blue]'{CurrentState.KubeContext}'[/]");

        return true;
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
}
