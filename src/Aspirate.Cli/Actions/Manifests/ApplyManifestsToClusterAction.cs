namespace Aspirate.Cli.Actions.Manifests;

public sealed class ApplyManifestsToClusterAction(IKubeCtlService kubeCtlService, IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "ApplyManifestsToClusterAction";

    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteLine();
        var shouldDeploy = Logger.Confirm("[bold]Would you like to deploy the generated manifests to a kubernetes cluster defined in your kubeconfig file?[/]");
        if (!shouldDeploy)
        {
            Logger.MarkupLine("[yellow]Cancelled![/]");

            return true;
        }

        CurrentState.ComputedParameters.ActiveKubernetesContext = await kubeCtlService.SelectKubernetesContextForDeployment();

        if (!CurrentState.ComputedParameters.ActiveKubernetesContextIsSet)
        {
            return false;
        }

        await kubeCtlService.ApplyManifests(CurrentState.ComputedParameters.ActiveKubernetesContext, CurrentState.ComputedParameters.KustomizeManifestPath);
        Logger.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments successfully applied to cluster [blue]'{CurrentState.ComputedParameters.ActiveKubernetesContext}'[/]");

        return true;
    }
}
