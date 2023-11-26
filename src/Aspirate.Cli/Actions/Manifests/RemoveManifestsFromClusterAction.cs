namespace Aspirate.Cli.Actions.Manifests;

public sealed class RemoveManifestsFromClusterAction(IKubeCtlService kubeCtlService, IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "RemoveManifestsFromClusterAction";

    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteLine();
        var shouldDeploy = Logger.Confirm("[bold]Would you like to remove the deployed manifests from a kubernetes cluster defined in your kubeconfig file?[/]");
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

        await kubeCtlService.RemoveManifests(CurrentState.ActiveKubernetesContext, CurrentState.InputPath);
        Logger.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments removed from cluster [blue]'{CurrentState.ActiveKubernetesContext}'[/]");

        return true;
    }
}
