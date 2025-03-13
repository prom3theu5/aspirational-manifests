namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : BaseCommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected abstract bool CommandUnlocksSecrets { get; }
    protected virtual bool CommandSkipsStateAndSecrets => false;
    protected virtual bool CommandAlwaysRequiresState => false;

    protected BaseCommand(string name, string description)
        : base(name, description)
    {
        AddOption(NonInteractiveOption.Instance);
        AddOption(DisableSecretsOption.Instance);
        AddOption(DisableStateOption.Instance);
        AddOption(LaunchProfileOption.Instance);
        Handler = CommandHandler.Create<TOptions, IServiceCollection>(ConstructCommand);
    }

    private async Task<int> ConstructCommand(TOptions options, IServiceCollection services)
    {
        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(services.BuildServiceProvider());

        var versionCheckService = handler.Services.GetRequiredService<IVersionCheckService>();
        await versionCheckService.CheckVersion();

        if (CommandSkipsStateAndSecrets)
        {
            handler.CurrentState.PopulateStateFromOptions(options);
            return await handler.HandleAsync(options);
        }

        var stateService = handler.Services.GetRequiredService<IStateService>();
        var secretService = handler.Services.GetRequiredService<ISecretService>();

        var stateOptions = GetStateManagementOptions(options, handler, CommandAlwaysRequiresState);

        await stateService.RestoreState(stateOptions);

        handler.CurrentState.PopulateStateFromOptions(options);

        LoadSecrets(options, secretService, handler);

        var exitCode = await handler.HandleAsync(options);

        await stateService.SaveState(stateOptions);

        return exitCode;
    }

    private void LoadSecrets(TOptions options, ISecretService secretService, TOptionsHandler handler) =>
        secretService.LoadSecrets(new SecretManagementOptions
        {
            DisableSecrets = handler.CurrentState.DisableSecrets,
            NonInteractive = options.NonInteractive,
            SecretPassword = options.SecretPassword,
            CommandUnlocksSecrets = CommandUnlocksSecrets,
            State = handler.CurrentState,
        });

    private static StateManagementOptions GetStateManagementOptions(TOptions options, TOptionsHandler handler, bool requiresState) =>
        new()
        {
            NonInteractive = options.NonInteractive,
            DisableState = options.DisableState,
            State = handler.CurrentState,
            RequiresState = requiresState,
        };
}
