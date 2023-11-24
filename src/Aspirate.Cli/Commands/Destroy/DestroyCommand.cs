namespace Aspirate.Cli.Commands.Destroy;

/// <summary>
/// The command to roll back an apply.
/// </summary>
public sealed class DestroyCommand(
    IAnsiConsole console,
    IKubeCtlService kubeCtlService,
    IServiceProvider serviceProvider) : AsyncCommand<DestroyInput>
{
    public const string CommandName = "destroy";
    public const string CommandDescription = "Removes the manifests from your cluster.";

    public override async Task<int> ExecuteAsync(CommandContext context, DestroyInput settings)
    {
        await HandleDeployment(settings);

        console.LogCommandCompleted();

        return 0;
    }

    private async Task HandleDeployment(DestroyInput settings)
    {
        console.WriteLine();
        var shouldDeploy = console.Confirm("[bold]Would you like to remove the deployed manifests from a kubernetes cluster defined in your kubeconfig file?[/]");
        if (!shouldDeploy)
        {
            return;
        }

        var successfullySelectedCluster = await kubeCtlService.SelectKubernetesContextForDeployment();

        if (!successfullySelectedCluster)
        {
            return;
        }

        await kubeCtlService.RemoveManifests(settings.OutputPathFlag);
        console.LogDestroyCommandCompleted(kubeCtlService.GetActiveContextName());
    }
}
