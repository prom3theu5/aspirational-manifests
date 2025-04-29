namespace Aspirate.Commands.Actions.BindMounts;
public sealed class ApplyMinikubeMountsAction(
    IServiceProvider serviceProvider,
    IMinikubeCliService minikubeCliService) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling minikube mounts[/]");
        await HandleMinikubeMounts();

        return true;
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            Logger.ValidationFailed("Wont start minikube mounts as kubecontext is not set");
        }
    }
    private async Task HandleMinikubeMounts()
    {
        if (CurrentState.KubeContext != "minikube" || CurrentState.EnableMinikubeMountAction.Equals(false) || CurrentState.BindMounts == null)
        {
            return;
        }

        Logger.MarkupLine("Applying volume mounts to minikube...");

        var minikubeCliInstalled = minikubeCliService.IsMinikubeCliInstalledOnMachine();

        if (!minikubeCliInstalled)
        {
            Logger.MarkupLine("[yellow]Minikube cli is required to perform Minikube volume mounts.[/]");
            Logger.MarkupLine("[yellow]Please install minikube cli following the guide here:[blue]https://minikube.sigs.k8s.io/docs/start/?arch=%2Fwindows%2Fx86-64%2Fstable%2F.exe+download/[/][/]");
            Logger.MarkupLine("[yellow]Manifest deployment will continue, but Minikube volume mounts will not be applied by aspirate.[/]");
            return;
        }

        await minikubeCliService.ActivateMinikubeMount(CurrentState);
    }
}
