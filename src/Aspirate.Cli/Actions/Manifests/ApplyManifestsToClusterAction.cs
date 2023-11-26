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

        CurrentState.ActiveKubernetesContext = await kubeCtlService.SelectKubernetesContextForDeployment();

        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            return false;
        }

        await kubeCtlService.ApplyManifests(CurrentState.ActiveKubernetesContext, CurrentState.InputPath);
        Logger.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments successfully applied to cluster [blue]'{CurrentState.ActiveKubernetesContext}'[/]");

        return true;
    }
}
