namespace Aspirate.Commands.Actions.KubeContext;
public sealed class SelectKubeContextAction(
    IServiceProvider serviceProvider,
    IKubernetesService kubernetesServiceClient) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling kubecontext[/]");
        await kubernetesServiceClient.InteractivelySelectKubernetesCluster(CurrentState);

        return true;
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            Logger.ValidationFailed("Cannot apply manifests to cluster without specifying the kubernetes context to use.");
        }
    }
}
