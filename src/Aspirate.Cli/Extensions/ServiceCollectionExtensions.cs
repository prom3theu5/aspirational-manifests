namespace Aspirate.Cli.Extensions;
internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddSerilogLogging()
            .AddFileParserSupport()
            .AddContainerDetailsSupport()
            .AddHandlers();

    private static IServiceCollection AddFileParserSupport(this IServiceCollection services) =>
        services
            .AddScoped<IFileSystem, FileSystem>()
            .AddScoped<IManifestFileParserService, ManifestFileParserService>();

    private static IServiceCollection AddContainerDetailsSupport(this IServiceCollection services) =>
        services
            .AddScoped<IProjectPropertyService, ProjectPropertyService>()
            .AddScoped<IContainerDetailsService, ContainerDetailsService>();

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services) =>
        services.AddLogging(ConfigureLogging);

    private static IServiceCollection AddHandlers(this IServiceCollection services) =>
        services
            .AddKeyedScoped<IProcessor, PostgresServerProcessor>(AspireResourceLiterals.PostgresServer)
            .AddKeyedScoped<IProcessor, PostgresDatabaseProcessor>(AspireResourceLiterals.PostgresDatabase)
            .AddKeyedScoped<IProcessor, ProjectProcessor>(AspireResourceLiterals.Project)
            .AddKeyedScoped<IProcessor, RedisProcessor>(AspireResourceLiterals.Redis)
            .AddKeyedScoped<IProcessor, FinalProcessor>(AspireResourceLiterals.Final);

    private static void ConfigureLogging(ILoggingBuilder builder)
    {
        var serilogConfig = new LoggerConfiguration()
            .WriteTo.Console()
#if DEBUG
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Warning()
#endif
            .CreateLogger();

        builder.ClearProviders();
        builder.AddSerilog(serilogConfig);
    }
}
