namespace Aspirate.Cli.Extensions;
internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspirateEssential(this IServiceCollection services)
    {
        services
            .AddFileParserSupport()
            .AddProjectPropertySupport()
            .AddHandlers();

        return services;
    }

    private static IServiceCollection AddFileParserSupport(this IServiceCollection services)
    {
        services.AddScoped<IFileSystem, FileSystem>();
        services.AddScoped<IManifestFileParserService, ManifestFileParserService>();

        return services;
    }

    private static IServiceCollection AddProjectPropertySupport(this IServiceCollection services)
    {
        services.AddScoped<IProjectPropertyService, ProjectPropertyService>();

        return services;
    }

    private static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddKeyedScoped<IProcessor, PostgresServerProcessor>(AspireResourceLiterals.PostgresServer);
        services.AddKeyedScoped<IProcessor, PostgresDatabaseProcessor>(AspireResourceLiterals.PostgresDatabase);
        services.AddKeyedScoped<IProcessor, ProjectProcessor>(AspireResourceLiterals.Project);
        services.AddKeyedScoped<IProcessor, FinalProcessor>(AspireResourceLiterals.Final);

        return services;
    }
}
