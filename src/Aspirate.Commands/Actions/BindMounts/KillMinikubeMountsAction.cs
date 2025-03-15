using System.Reflection.Metadata.Ecma335;
using Aspirate.Shared.Models.AspireManifests.Components.Common;
using Aspirate.Shared.Models.AspireManifests.Components.Common.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Aspirate.Commands.Actions.BindMounts;
public sealed class KillMinikubeMountsAction(
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
        if (minikubeCliService.IsMinikubeCliInstalledOnMachine() && (CurrentState.DisableMinikubeMountAction.Equals(false) || !CurrentState.DisableMinikubeMountAction.HasValue))
        {
            minikubeCliService.KillMinikubeMounts(CurrentState);
            Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Killed minikube mount processes [blue][/]");
        }
    }
}
