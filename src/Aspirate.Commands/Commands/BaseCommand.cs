namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : BaseCommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description)
        : base(name, description)
    {
        AddOption(NonInteractiveOption.Instance);
        AddOption(SecretProviderOption.Instance);
        AddOption(DisableSecretsOption.Instance);
        Handler = CommandHandler.Create<TOptions, IServiceCollection>(ConstructCommand);
    }

    private static async Task<int> ConstructCommand(TOptions options, IServiceCollection services)
    {
        services.RegisterAspirateSecretProvider(options.SecretProvider);

        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(services.BuildServiceProvider());

        var stateOptions = GetStateManagementOptions(options, handler);

        var stateService = handler.Services.GetRequiredService<IStateService>();
        await stateService.RestoreState(stateOptions);

        handler.CurrentState.PopulateStateFromOptions(options);

        LoadSecrets(options, handler);

        var exitCode = await handler.HandleAsync(options);

        await stateService.SaveState(stateOptions);

        return exitCode;
    }

    private static void LoadSecrets(TOptions options, TOptionsHandler handler)
    {
        var secretService = handler.Services.GetRequiredService<ISecretService>();

        secretService.LoadSecrets(new SecretManagementOptions
        {
            DisableSecrets = options.DisableSecrets,
            NonInteractive = options.NonInteractive,
            SecretPassword = options.SecretPassword,
            State = handler.CurrentState,
        });
    }

    private static StateManagementOptions GetStateManagementOptions(TOptions options, TOptionsHandler handler) =>
        new()
        {
            NonInteractive = options.NonInteractive,
            DisableState = options.DisableState,
            State = handler.CurrentState
        };
}
