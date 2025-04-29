namespace Aspirate.Commands.Actions.KubeContext;
public sealed class SelectKubeContextAction(
    IServiceProvider serviceProvider,
    IKubernetesService kubernetesServiceClient) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling kubecontext[/]");

        if (CurrentState.NonInteractive && !CurrentState.ActiveKubernetesContextIsSet)
        {
            return true;
        }

        await kubernetesServiceClient.InteractivelySelectKubernetesCluster(CurrentState);

        return true;
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet && (CurrentState.CurrentCommand == AspirateLiterals.ApplyCommand || CurrentState.CurrentCommand == AspirateLiterals.RunCommand))
        {
            Logger.ValidationFailed("Cannot apply manifests to cluster without specifying the kubernetes context to use.");
        }
    }
}
