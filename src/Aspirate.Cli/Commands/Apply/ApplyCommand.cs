namespace Aspirate.Cli.Commands.Apply;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
public sealed class ApplyCommand(AspirateState currentState, IServiceProvider serviceProvider) : AsyncCommand<ApplyInput>
{
    public const string CommandName = "apply";
    public const string CommandDescription = "Deployes the manifests to the kubernetes context after asking which you'd like to use.";

    public override async Task<int> ExecuteAsync(CommandContext context, ApplyInput settings)
    {
        currentState.ComputedParameters.SetKustomizeManifestPath(settings.OutputPathFlag);

        var actionExecutor = ActionExecutor.CreateInstance(serviceProvider);

        await actionExecutor
            .QueueAction(ApplyManifestsToClusterAction.ActionKey)
            .ExecuteCommandsAsync();

        return 0;
    }
}
