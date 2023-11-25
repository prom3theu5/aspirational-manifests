namespace Aspirate.Cli.Commands.Init;

public sealed class InitCommand : BaseCommand<InitOptions, InitCommandHandler>
{
    public InitCommand() : base("init", "Initializes aspirate settings within your AppHost directory.") =>
        AddOption(new Option<string>(new[] { "-p", "--project-path" })
        {
            Description = "The path to the aspire project",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });
}

public class InitCommandHandler(AspirateState currentState, IServiceProvider serviceProvider) : ICommandOptionsHandler<InitOptions>
{
    public async Task<int> HandleAsync(InitOptions options, CancellationToken cancellationToken)
    {
        currentState.InputParameters.AspireManifestPath = options.ProjectPath;

        var actionExecutor = ActionExecutor.CreateInstance(serviceProvider);

        await actionExecutor
            .QueueAction(InitializeConfigurationAction.ActionKey)
            .ExecuteCommandsAsync();

        return 0;
    }
}
