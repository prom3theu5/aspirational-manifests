namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<ApplyOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(ApplyOptions options, CancellationToken cancellationToken)
    {
        CurrentState.ComputedParameters.SetKustomizeManifestPath(options.OutputPath);

        return ActionExecutor
            .QueueAction(ApplyManifestsToClusterAction.ActionKey)
            .ExecuteCommandsAsync();
    }
}
