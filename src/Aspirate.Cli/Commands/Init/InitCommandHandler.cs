namespace Aspirate.Cli.Commands.Init;

public class InitCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<InitOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(InitOptions options, CancellationToken cancellationToken)
    {
        CurrentState.InputParameters.AspireManifestPath = options.ProjectPath;

        return ActionExecutor
            .QueueAction(InitializeConfigurationAction.ActionKey)
            .ExecuteCommandsAsync();
    }
}
