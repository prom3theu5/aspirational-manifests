namespace Aspirate.Cli.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddSpectreConsole()
            .AddAspirateState()
            .AddAspireManifestSupport()
            .AddAspirateConfigurationSupport()
            .AddContainerSupport()
            .AddKubeCtlSupport()
            .AddActions()
            .AddProcessors();

    private static IServiceCollection AddSpectreConsole(this IServiceCollection services) =>
        services.AddSingleton(AnsiConsole.Console);

    public static IServiceCollection AddAspirateState(this IServiceCollection services) =>
        services.AddSingleton<AspirateState>();

    private static IServiceCollection AddAspireManifestSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IAspireManifestCompositionService, AspireManifestCompositionService>()
            .AddSingleton<IManifestFileParserService, ManifestFileParserService>();

    private static IServiceCollection AddAspirateConfigurationSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IAspirateConfigurationService, AspirateConfigurationService>();

    private static IServiceCollection AddKubeCtlSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IKubeCtlService, KubeCtlService>();

    private static IServiceCollection AddContainerSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IProjectPropertyService, ProjectPropertyService>()
            .AddSingleton<IContainerCompositionService, ContainerCompositionService>()
            .AddSingleton<IContainerDetailsService, ContainerDetailsService>();

    private static IServiceCollection AddActions(this IServiceCollection services) =>
        services
            .AddKeyedSingleton<IAction, InitializeConfigurationAction>(InitializeConfigurationAction.ActionKey)
            .AddKeyedSingleton<IAction, LoadConfigurationAction>(LoadConfigurationAction.ActionKey)
            .AddKeyedSingleton<IAction, BuildAndPushContainersAction>(BuildAndPushContainersAction.ActionKey)
            .AddKeyedSingleton<IAction, PopulateContainerDetailsAction>(PopulateContainerDetailsAction.ActionKey)
            .AddKeyedSingleton<IAction, GenerateAspireManifestAction>(GenerateAspireManifestAction.ActionKey)
            .AddKeyedSingleton<IAction, GenerateKustomizeManifestAction>(GenerateKustomizeManifestAction.ActionKey)
            .AddKeyedSingleton<IAction, LoadAspireManifestAction>(LoadAspireManifestAction.ActionKey)
            .AddKeyedSingleton<IAction, ApplyManifestsToClusterAction>(ApplyManifestsToClusterAction.ActionKey)
            .AddKeyedSingleton<IAction, RemoveManifestsFromClusterAction>(RemoveManifestsFromClusterAction.ActionKey);

    private static void AddProcessors(this IServiceCollection services) =>
        services
            .AddKeyedSingleton<IProcessor, PostgresServerProcessor>(AspireLiterals.PostgresServer)
            .AddKeyedSingleton<IProcessor, PostgresDatabaseProcessor>(AspireLiterals.PostgresDatabase)
            .AddKeyedSingleton<IProcessor, ProjectProcessor>(AspireLiterals.Project)
            .AddKeyedSingleton<IProcessor, RedisProcessor>(AspireLiterals.Redis)
            .AddKeyedSingleton<IProcessor, RabbitMqProcessor>(AspireLiterals.RabbitMq)
            .AddKeyedSingleton<IProcessor, FinalProcessor>(AspireLiterals.Final);
}
