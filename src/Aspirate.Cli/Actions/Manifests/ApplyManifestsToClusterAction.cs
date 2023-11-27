namespace Aspirate.Cli.Actions.Manifests;

public sealed class ApplyManifestsToClusterAction(IKubeCtlService kubeCtlService, IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public const string ActionKey = "ApplyManifestsToClusterAction";

    public override async Task<bool> ExecuteAsync()
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

        await kubeCtlService.ApplyManifests(CurrentState.KubeContext, CurrentState.InputPath);
        Logger.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments successfully applied to cluster [blue]'{CurrentState.KubeContext}'[/]");

        return true;
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
}
