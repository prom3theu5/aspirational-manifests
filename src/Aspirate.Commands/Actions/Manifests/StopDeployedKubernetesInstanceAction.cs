namespace Aspirate.Commands.Actions.Manifests;

public sealed class StopDeployedKubernetesInstanceAction(
    IKubernetesService kubernetesService,
    IKustomizeService kustomizeService,
    ISecretProvider secretProvider,
    IFileSystem fileSystem,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Stopping Deployment in Cluster[/]");

        if (CurrentState.IsRunning != true)
        {
            Logger.MarkupLine("[bold]No deployment is currently running according to state. Are you sure you ran a deployment?[/]");
            return false;
        }

        if (string.IsNullOrEmpty(CurrentState.KubeContext))
        {
            Logger.MarkupLine("[bold]No Kubernetes context is set in state. Are you sure you ran a deployment?[/]");
            return false;
        }

        var client = kubernetesService.CreateClient(CurrentState.KubeContext);

        var options = new KubernetesRunOptions
        {
            Client = client,
            KubernetesObjects = [],
            NamespaceName = string.IsNullOrEmpty(CurrentState.Namespace) ? "default" : CurrentState.Namespace,
            CurrentState = CurrentState
        };

        var namespaceIsClean = await kubernetesService.ClearNamespace(options);
        if (!namespaceIsClean)
        {
            return false;
        }

        var namespaceIsDeleted = await kubernetesService.DeleteNamespace(options);
        if (!namespaceIsDeleted)
        {
            return false;
        }

        CurrentState.IsRunning = false;

        return true;
    }
}
