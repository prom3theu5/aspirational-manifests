namespace Aspirate.Cli.Extensions;
internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddSpectreConsole()
            .AddAspireManifestSupport()
            .AddAspirateConfigurationSupport()
            .AddContainerSupport()
            .AddProcessors();

    private static IServiceCollection AddSpectreConsole(this IServiceCollection services) =>
        services.AddSingleton(AnsiConsole.Console);

    private static IServiceCollection AddAspireManifestSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IAspireManifestCompositionService, AspireManifestCompositionService>()
            .AddSingleton<IManifestFileParserService, ManifestFileParserService>();

    private static IServiceCollection AddAspirateConfigurationSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IAspirateConfigurationService, AspirateConfigurationService>();

    private static IServiceCollection AddContainerSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IProjectPropertyService, ProjectPropertyService>()
            .AddSingleton<IContainerCompositionService, ContainerCompositionService>()
            .AddSingleton<IContainerDetailsService, ContainerDetailsService>();

    private static IServiceCollection AddProcessors(this IServiceCollection services) =>
        services
            .AddKeyedSingleton<IProcessor, PostgresServerProcessor>(AspireLiterals.PostgresServer)
            .AddKeyedSingleton<IProcessor, PostgresDatabaseProcessor>(AspireLiterals.PostgresDatabase)
            .AddKeyedSingleton<IProcessor, ProjectProcessor>(AspireLiterals.Project)
            .AddKeyedSingleton<IProcessor, RedisProcessor>(AspireLiterals.Redis)
            .AddKeyedSingleton<IProcessor, RabbitMqProcessor>(AspireLiterals.RabbitMq)
            .AddKeyedSingleton<IProcessor, FinalProcessor>(AspireLiterals.Final);
}
