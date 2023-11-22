namespace Aspirate.Cli.Extensions;
internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddAspireManifestSupport()
            .AddContainerSupport()
            .AddHandlers();

    private static IServiceCollection AddAspireManifestSupport(this IServiceCollection services) =>
        services
            .AddScoped<IFileSystem, FileSystem>()
            .AddScoped<IAspireManifestCompositionService, AspireManifestCompositionService>()
            .AddScoped<IManifestFileParserService, ManifestFileParserService>();

    private static IServiceCollection AddContainerSupport(this IServiceCollection services) =>
        services
            .AddScoped<IProjectPropertyService, ProjectPropertyService>()
            .AddScoped<IContainerCompositionService, ContainerCompositionService>()
            .AddScoped<IContainerDetailsService, ContainerDetailsService>();

    private static IServiceCollection AddHandlers(this IServiceCollection services) =>
        services
            .AddKeyedScoped<IProcessor, PostgresServerProcessor>(AspireLiterals.PostgresServer)
            .AddKeyedScoped<IProcessor, PostgresDatabaseProcessor>(AspireLiterals.PostgresDatabase)
            .AddKeyedScoped<IProcessor, ProjectProcessor>(AspireLiterals.Project)
            .AddKeyedScoped<IProcessor, RedisProcessor>(AspireLiterals.Redis)
            .AddKeyedScoped<IProcessor, RabbitMqProcessor>(AspireLiterals.RabbitMq)
            .AddKeyedScoped<IProcessor, FinalProcessor>(AspireLiterals.Final);
}
