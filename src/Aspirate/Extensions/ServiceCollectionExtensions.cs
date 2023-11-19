namespace Aspirate.Extensions;
internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspirateEssential(this IServiceCollection services)
    {
        services
            .AddFileParserSupport()
            .AddHandlers();

        return services;
    }

    private static IServiceCollection AddFileParserSupport(this IServiceCollection services)
    {
        services.AddScoped<IFileSystem, FileSystem>();
        services.AddScoped<IManifestFileParserService, ManifestFileParserService>();

        return services;
    }

    private static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddKeyedScoped<IHandler, PostgresServerHandler>(AspireResourceLiterals.PostgresServer);
        services.AddKeyedScoped<IHandler, PostgresDatabaseHandler>(AspireResourceLiterals.PostgresDatabase);
        services.AddKeyedScoped<IHandler, ProjectHandler>(AspireResourceLiterals.Project);
        services.AddKeyedScoped<IHandler, FinalHandler>(AspireResourceLiterals.Final);

        return services;
    }
}
