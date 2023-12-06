using Aspirate.Commands;
using Aspirate.Services.Implementations;

namespace Aspirate.Cli.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static void RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddSpectreConsole()
            .AddAspirateState()
            .AddShellExecution()
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

    private static IServiceCollection AddShellExecution(this IServiceCollection services) =>
        services.AddSingleton<IShellExecutionService, ShellExecutionService>();

    private static IServiceCollection AddContainerSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IProjectPropertyService, ProjectPropertyService>()
            .AddSingleton<IContainerCompositionService, ContainerCompositionService>()
            .AddSingleton<IContainerDetailsService, ContainerDetailsService>();

    private static IServiceCollection AddActions(this IServiceCollection services) =>
        services
            .RegisterAction<InitializeConfigurationAction>()
            .RegisterAction<LoadConfigurationAction>()
            .RegisterAction<AskImagePullPolicyAction>()
            .RegisterAction<BuildAndPushContainersFromProjectsAction>()
            .RegisterAction<BuildAndPushContainersFromDockerfilesAction>()
            .RegisterAction<PopulateContainerDetailsForProjectsAction>()
            .RegisterAction<GenerateAspireManifestAction>()
            .RegisterAction<GenerateKustomizeManifestsAction>()
            .RegisterAction<GenerateFinalKustomizeManifestAction>()
            .RegisterAction<LoadAspireManifestAction>()
            .RegisterAction<ApplyManifestsToClusterAction>()
            .RegisterAction<RemoveManifestsFromClusterAction>();

    private static void AddProcessors(this IServiceCollection services) =>
        services
            .RegisterProcessor<PostgresServerProcessor>(AspireLiterals.PostgresServer)
            .RegisterProcessor<PostgresDatabaseProcessor>(AspireLiterals.PostgresDatabase)
            .RegisterProcessor<ProjectProcessor>(AspireLiterals.Project)
            .RegisterProcessor<DockerfileProcessor>(AspireLiterals.Dockerfile)
            .RegisterProcessor<RedisProcessor>(AspireLiterals.Redis)
            .RegisterProcessor<RabbitMqProcessor>(AspireLiterals.RabbitMq)
            .RegisterProcessor<FinalProcessor>(AspireLiterals.Final);

    private static IServiceCollection RegisterAction<TImplementation>(this IServiceCollection services) where TImplementation : class, IAction =>
        services.AddKeyedSingleton<IAction, TImplementation>(typeof(TImplementation).Name);

    private static IServiceCollection RegisterProcessor<TImplementation>(this IServiceCollection services, string key) where TImplementation : class, IProcessor =>
        services.AddKeyedSingleton<IProcessor, TImplementation>(key);
}
