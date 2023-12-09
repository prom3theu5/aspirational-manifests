namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<ApplyOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(ApplyOptions options) =>
        ActionExecutor
            .QueueAction(nameof(LoadSecretsAction))
            .QueueAction(nameof(ApplyManifestsToClusterAction))
            .ExecuteCommandsAsync();
}
