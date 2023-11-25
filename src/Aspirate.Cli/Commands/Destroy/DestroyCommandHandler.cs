namespace Aspirate.Cli.Commands.Destroy;

public sealed class DestroyCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<DestroyOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(DestroyOptions options, CancellationToken cancellationToken)
    {
        CurrentState.ComputedParameters.SetKustomizeManifestPath(options.InputPath);

        return ActionExecutor
            .QueueAction(RemoveManifestsFromClusterAction.ActionKey)
            .ExecuteCommandsAsync();
    }
}
