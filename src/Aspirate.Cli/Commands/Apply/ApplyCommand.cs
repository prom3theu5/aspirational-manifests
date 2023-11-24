namespace Aspirate.Cli.Commands.Apply;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
public sealed class ApplyCommand(
    IAnsiConsole console,
    IKubeCtlService kubeCtlService,
    IServiceProvider serviceProvider) : AsyncCommand<ApplyInput>
{
    public const string CommandName = "apply";
    public const string CommandDescription = "Deployes the manifests to the kubernetes context after asking which you'd like to use.";

    public override async Task<int> ExecuteAsync(CommandContext context, ApplyInput settings)
    {
        await HandleDeployment(settings);

        console.LogCommandCompleted();

        return 0;
    }

    private async Task HandleDeployment(ApplyInput settings)
    {
        console.WriteLine();
        var shouldDeploy = console.Confirm("[bold]Would you like to deploy the generated manifests to a kubernetes cluster defined in your kubeconfig file?[/]");
        if (!shouldDeploy)
        {
            return;
        }

        var successfullySelectedCluster = await kubeCtlService.SelectKubernetesContextForDeployment();

        if (!successfullySelectedCluster)
        {
            return;
        }

        await kubeCtlService.ApplyManifests(settings.OutputPathFlag);
        console.LogApplyCommandCompleted(kubeCtlService.GetActiveContextName());
    }
}
