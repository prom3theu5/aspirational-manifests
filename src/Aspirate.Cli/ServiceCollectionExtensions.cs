namespace Aspirate.Cli;

internal static class ServiceCollectionExtensions
{
    internal static void RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddSpectreConsole()
            .AddSecretProtectionStrategies()
            .AddAspirateState()
            .AddAspirateServices()
            .AddAspirateActions()
            .AddAspirateProcessors()
            .AddAspirateSecretProvider()
            .AddPlaceholderTransformation();

    private static IServiceCollection AddSpectreConsole(this IServiceCollection services) =>
        services.AddSingleton(AnsiConsole.Console);
}
