namespace Aspirate.Commands.Commands.Stop;

public sealed class StopCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<StopOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(StopOptions options) =>
        ActionExecutor
            .QueueAction(nameof(StopDeployedKubernetesInstanceAction))
            .ExecuteCommandsAsync();
}
