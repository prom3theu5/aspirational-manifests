using System.Reflection.Metadata.Ecma335;
using Aspirate.Shared.Models.AspireManifests.Components.Common;
using Aspirate.Shared.Models.AspireManifests.Components.Common.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Aspirate.Commands.Actions.BindMounts;
public sealed class ApplyMinikubeMountsAction(
    IServiceProvider serviceProvider,
    IMinikubeCliService minikubeCliService) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling minikube mounts[/]");
        await Task.Run(HandleMinikubeMounts);

        return true;
    }
    private void HandleMinikubeMounts()
    {
        if (CurrentState.KubeContext != "minikube" || CurrentState.DisableMinikubeMountAction.Equals(true))
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

        minikubeCliService.ActivateMinikubeMount(CurrentState);
    }
}
