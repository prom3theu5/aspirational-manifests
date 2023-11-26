namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<ApplyOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(ApplyOptions options) =>
        ActionExecutor
            .QueueAction(ApplyManifestsToClusterAction.ActionKey)
            .ExecuteCommandsAsync();
}
