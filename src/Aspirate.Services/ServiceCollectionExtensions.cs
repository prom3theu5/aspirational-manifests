namespace Aspirate.Services;

/// <summary>
/// Extension methods for IServiceCollection to add Aspirate services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the required Aspirate services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the added services.</returns>
    public static IServiceCollection AddAspirateServices(this IServiceCollection services) =>
        services
            .AddShellExecution()
            .AddPasswordGenerator()
            .AddAspireManifestSupport()
            .AddAspirateConfigurationSupport()
            .AddContainerSupport()
            .AddDaprCliSupport()
            .AddMinikubeCliSupport()
            .AddKubernetesSupport()
            .AddStateManagement()
            .AddSecretService()
            .AddVersionChecks()
            .AddProcessService();

    /// <summary>
    /// Adds the password generator implementation to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the password generator to.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddPasswordGenerator(this IServiceCollection services) =>
        services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

    /// <summary>
    /// Adds Aspire manifest support to the <see cref="IServiceCollection"/> container. </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the Aspire manifest support to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// /
    private static IServiceCollection AddAspireManifestSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IAspireManifestCompositionService, AspireManifestCompositionService>()
            .AddSingleton<IManifestFileParserService, ManifestFileParserService>()
            .AddSingleton<IManifestWriter, ManifestWriter>();

    private static IServiceCollection AddKubernetesSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IKubeCtlService, KubeCtlService>()
            .AddSingleton<IKustomizeService, KustomizeService>()
            .AddSingleton<IKubernetesService, KubernetesService>()
            .AddSingleton<IHelmChartCreator, HelmChartCreator>();

    /// <summary>
    /// Adds the AspirateConfigurationSupport to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the support to.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddAspirateConfigurationSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IAspirateConfigurationService, AspirateConfigurationService>();

    /// <summary>
    /// Adds Dapr CLI support to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the Dapr CLI support to.</param>
    /// <returns>The updated service collection with Dapr CLI support added.</returns>
    private static IServiceCollection AddDaprCliSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IDaprCliService, DaprCliService>();

    private static IServiceCollection AddMinikubeCliSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IMinikubeCliService, MinikubeCliService>();

    private static IServiceCollection AddStateManagement(this IServiceCollection services) =>
        services
            .AddSingleton<IStateService, StateService>();

    private static IServiceCollection AddSecretService(this IServiceCollection services) =>
        services
            .AddSingleton<ISecretService, SecretService>();

    private static IServiceCollection AddVersionChecks(this IServiceCollection services) =>
        services
            .AddSingleton<IVersionCheckService, VersionCheckService>();

    /// <summary>
    /// Adds the shell execution service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the shell execution service to.</param>
    /// <returns>The modified service collection.</returns>
    private static IServiceCollection AddShellExecution(this IServiceCollection services) =>
        services.AddSingleton<IShellExecutionService, ShellExecutionService>();

    private static IServiceCollection AddProcessService(this IServiceCollection services) =>
        services.AddSingleton<IProcessService, ProcessService>();

    /// <summary>
    /// Adds container support to the IServiceCollection. </summary> <param name="services">The IServiceCollection to add the container support to.</param> <returns>The modified IServiceCollection with the added container support.</returns>
    /// /
    private static IServiceCollection AddContainerSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IProjectPropertyService, ProjectPropertyService>()
            .AddSingleton<IContainerCompositionService, ContainerCompositionService>()
            .AddSingleton<IContainerDetailsService, ContainerDetailsService>();
}
