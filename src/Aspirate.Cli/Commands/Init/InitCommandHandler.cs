namespace Aspirate.Cli.Commands.Init;

public class InitCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<InitOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(InitOptions options) =>
        ActionExecutor
            .QueueAction(InitializeConfigurationAction.ActionKey)
            .ExecuteCommandsAsync();
}
