namespace Aspirate.Commands.Actions.Manifests;

public class CustomNamespaceAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Namespace[/]");

        if (CurrentState.UseCustomNamespace != null)
        {
            if (CurrentState.UseCustomNamespace == true && !string.IsNullOrEmpty(CurrentState.Namespace))
            {
                Logger.MarkupLine($"[green]Generated manifests will be deployed to the [bold]{CurrentState.Namespace}[/] namespace.[/]");
                return Task.FromResult(true);
            }

            Logger.MarkupLine($"[green]Generated manifests will be deployed to the [bold]default[/] namespace.[/]");
            return Task.FromResult(true);
        }

        if (!string.IsNullOrEmpty(CurrentState.Namespace) || CurrentState.NonInteractive)
        {
            CurrentState.UseCustomNamespace = true;
            Logger.MarkupLine($"[green]Generated manifests will be deployed to the [bold]{CurrentState.Namespace}[/] namespace.[/]");
            return Task.FromResult(true);
        }

        AskCustomNamespace();

        return Task.FromResult(true);
    }

    private void AskCustomNamespace()
    {
        var shouldDeployCustomNamespace = Logger.Confirm(
            "[bold]Would you like to deploy all manifests to a custom namespace?[/]", false);

        CurrentState.UseCustomNamespace = shouldDeployCustomNamespace;

        if (!shouldDeployCustomNamespace)
        {
            Logger.MarkupLine($"[green]Generated manifests will be deployed to the [bold]default[/] namespace.[/]");
            return;
        }

        CurrentState.Namespace = Logger.Ask<string>("[bold]Enter the namespace you would like to deploy all manifests to: [/]");

        Logger.MarkupLine($"[green] Generated manifests will be deployed to the [bold]{CurrentState.Namespace}[/] namespace.[/]");
    }
}
