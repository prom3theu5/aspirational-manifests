namespace Aspirate.Cli.Commands.Init;

public sealed class InitCommand(AspirateState currentState, IServiceProvider serviceProvider) : AsyncCommand<InitInput>
{
    public const string CommandName = "init";
    public const string CommandDescription = "Initializes aspirate settings within your AppHost directory.";

    public override async Task<int> ExecuteAsync(CommandContext context, InitInput settings)
    {
        currentState.InputParameters.AspireManifestPath = settings.PathToAspireProjectFlag;

        var actionExecutor = ActionExecutor.CreateInstance(serviceProvider);

        await actionExecutor
            .QueueAction(InitializeConfigurationAction.ActionKey)
            .ExecuteCommandsAsync();

        return 0;
    }
}
