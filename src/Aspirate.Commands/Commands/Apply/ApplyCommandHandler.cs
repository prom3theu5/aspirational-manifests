using Aspirate.Commands.Actions.BindMounts;

namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<ApplyOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(ApplyOptions optionses) =>
        ActionExecutor
            .QueueAction(nameof(ApplyMinikubeMountsAction))
            .QueueAction(nameof(ApplyManifestsToClusterAction))
            .ExecuteCommandsAsync();
}
