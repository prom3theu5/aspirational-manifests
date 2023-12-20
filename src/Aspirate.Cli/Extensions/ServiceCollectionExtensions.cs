namespace Aspirate.Cli.Extensions;

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
            .AddAspiratePlaceholderSubstitutionStrategies();

    private static IServiceCollection AddSpectreConsole(this IServiceCollection services) =>
        services.AddSingleton(AnsiConsole.Console);
}
