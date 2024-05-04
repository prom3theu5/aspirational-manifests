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

        var stateService = handler.Services.GetRequiredService<IStateService>();

        await stateService.RestoreState(handler.CurrentState);

        handler.CurrentState.PopulateStateFromOptions(options);

        var exitCode = await handler.HandleAsync(options);

        await stateService.SaveState(handler.CurrentState);

        return exitCode;
    }
}
