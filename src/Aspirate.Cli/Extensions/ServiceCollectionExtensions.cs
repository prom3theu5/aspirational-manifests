namespace Aspirate.Cli.Extensions;
internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddFileParserSupport()
            .AddContainerSupport()
            .AddHandlers();

    private static IServiceCollection AddFileParserSupport(this IServiceCollection services) =>
        services
            .AddScoped<IFileSystem, FileSystem>()
            .AddScoped<IManifestFileParserService, ManifestFileParserService>();

    private static IServiceCollection AddContainerSupport(this IServiceCollection services) =>
        services
            .AddScoped<IProjectPropertyService, ProjectPropertyService>()
            .AddScoped<IContainerCompositionService, ContainerCompositionService>()
            .AddScoped<IContainerDetailsService, ContainerDetailsService>();

    private static IServiceCollection AddHandlers(this IServiceCollection services) =>
        services
            .AddKeyedScoped<IProcessor, PostgresServerProcessor>(AspireResourceLiterals.PostgresServer)
            .AddKeyedScoped<IProcessor, PostgresDatabaseProcessor>(AspireResourceLiterals.PostgresDatabase)
            .AddKeyedScoped<IProcessor, ProjectProcessor>(AspireResourceLiterals.Project)
            .AddKeyedScoped<IProcessor, RedisProcessor>(AspireResourceLiterals.Redis)
            .AddKeyedScoped<IProcessor, RabbitMqProcessor>(AspireResourceLiterals.RabbitMq)
            .AddKeyedScoped<IProcessor, FinalProcessor>(AspireResourceLiterals.Final);
}
