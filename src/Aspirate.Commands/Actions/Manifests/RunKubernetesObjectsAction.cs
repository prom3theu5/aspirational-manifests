namespace Aspirate.Commands.Actions.Manifests;

public sealed class RunKubernetesObjectsAction(
    IKubernetesService kubernetesService,
    IKustomizeService kustomizeService,
    ISecretProvider secretProvider,
    IFileSystem fileSystem,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Running Against Cluster[/]");

        if (NoSupportedComponentsExitAction())
        {
            return true;
        }

        var kubeObjects = kubernetesService.ConvertResourcesToKubeObjects(CurrentState.AllSelectedSupportedComponents, CurrentState, false);
        var client = kubernetesService.CreateClient(CurrentState.KubeContext);

        var options = new KubernetesRunOptions
        {
            Client = client,
            KubernetesObjects = kubeObjects,
            NamespaceName = string.IsNullOrEmpty(CurrentState.Namespace) ? "default" : CurrentState.Namespace,
            CurrentState = CurrentState
        };

        var namespaceIsClean = await EnsureCleanNamespace(options);
        if (!namespaceIsClean)
        {
            return false;
        }

        var deployedSuccessfully = await kubernetesService.ApplyObjectsToCluster(options);

        if (!deployedSuccessfully)
        {
            Logger.MarkupLine("[bold]Failed to deploy Kubernetes objects to cluster.[/]");
            return false;
        }

        await kubernetesService.ListServiceAddresses(options);

        CurrentState.IsRunning = true;

        return true;
    }

    private async Task<bool> EnsureCleanNamespace(KubernetesRunOptions options)
    {
        var namespaceIsEmpty = await kubernetesService.IsNamespaceEmpty(options);

        if (namespaceIsEmpty)
        {
            return true;
        }

        if (CurrentState.AllowClearNamespace != true && CurrentState.NonInteractive)
        {
            Logger.MarkupLine($"[yellow]Skipping resource creation in namespace '{options.NamespaceName}' because it is not empty.[/]");
            return false;
        }

        if (CurrentState is { AllowClearNamespace: true })
        {
            return await ClearCurrentNamespace(options);
        }

        var shouldClearNamespace = Logger.Confirm($"Namespace '{options.NamespaceName}' is not empty. Would you like to clear it?", false);

        CurrentState.AllowClearNamespace = shouldClearNamespace;

        if (shouldClearNamespace)
        {
            return await ClearCurrentNamespace(options);
        }

        Logger.MarkupLine($"[yellow]Skipping resource creation in namespace '{options.NamespaceName}'[/]");
        CurrentState.AllowClearNamespace = false;
        return false;
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            Logger.ValidationFailed("Cannot run against a cluster without specifying which cluster to use.");
        }
    }

    private Task<bool> ClearCurrentNamespace(KubernetesRunOptions options)
    {
        CurrentState.AllowClearNamespace = true;
        Logger.MarkupLine($"[yellow]Clearing namespace '{options.NamespaceName}'[/]");
        return kubernetesService.ClearNamespace(options);
    }

    private bool NoSupportedComponentsExitAction()
    {
        if (CurrentState.HasSelectedSupportedComponents)
        {
            return false;
        }

        Logger.MarkupLine("[bold]No supported components selected. Kubernetes objects will not be deployed.[/]");
        return true;
    }
}
