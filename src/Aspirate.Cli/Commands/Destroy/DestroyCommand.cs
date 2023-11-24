namespace Aspirate.Cli.Commands.Destroy;

/// <summary>
/// The command to roll back an apply.
/// </summary>
public sealed class DestroyCommand(AspirateState currentState, IServiceProvider serviceProvider) : AsyncCommand<DestroyInput>
{
    public const string CommandName = "destroy";
    public const string CommandDescription = "Removes the manifests from your cluster.";

    public override async Task<int> ExecuteAsync(CommandContext context, DestroyInput settings)
    {
        currentState.ComputedParameters.SetKustomizeManifestPath(settings.OutputPathFlag);

        var actionExecutor = ActionExecutor.CreateInstance(serviceProvider);

        await actionExecutor
            .QueueAction(RemoveManifestsFromClusterAction.ActionKey)
            .ExecuteCommandsAsync();

        return 0;
    }
}
