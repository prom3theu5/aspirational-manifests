namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<ApplyOptionses>(serviceProvider)
{
    public override Task<int> HandleAsync(ApplyOptionses optionses) =>
        ActionExecutor
            .QueueAction(nameof(LoadSecretsAction))
            .QueueAction(nameof(ApplyManifestsToClusterAction))
            .ExecuteCommandsAsync();
}
