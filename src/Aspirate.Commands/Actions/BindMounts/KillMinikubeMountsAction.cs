namespace Aspirate.Commands.Actions.BindMounts;
public sealed class KillMinikubeMountsAction(
    IServiceProvider serviceProvider,
    IMinikubeCliService minikubeCliService) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling minikube mounts[/]");

        await HandleMinikubeMounts();

        return true;
    }
    private async Task HandleMinikubeMounts()
    {
        if (!CurrentState.KubeContext.Equals(MinikubeLiterals.Path, StringComparison.OrdinalIgnoreCase))
        {
            Logger.MarkupLine($"No minikube mounts to handle.");
            return;
        }

        if (minikubeCliService.IsMinikubeCliInstalledOnMachine() && CurrentState.EnableMinikubeMountAction.Equals(true) && CurrentState.BindMounts != null)
        {
            var result = await minikubeCliService.KillMinikubeMounts(CurrentState);
            if (result)
            {
                Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Killed minikube mount processes [blue][/]");
            }
            else
            {
                Logger.MarkupLine($"[red]({EmojiLiterals.Warning}) Done:[/] Could not kill all minikube mount processes [blue][/]");
            }
        }
    }
}
