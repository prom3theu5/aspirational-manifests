namespace Aspirate.Services.Extensions;

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
            .AddAspireManifestSupport()
            .AddAspirateConfigurationSupport()
            .AddContainerSupport()
            .AddKubeCtlSupport();

    /// <summary>
    /// Adds Aspire manifest support to the <see cref="IServiceCollection"/> container. </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the Aspire manifest support to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// /
    private static IServiceCollection AddAspireManifestSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IAspireManifestCompositionService, AspireManifestCompositionService>()
            .AddSingleton<IManifestFileParserService, ManifestFileParserService>();

    /// <summary>
    /// Adds the AspirateConfigurationSupport to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the support to.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddAspirateConfigurationSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IAspirateConfigurationService, AspirateConfigurationService>();

    /// <summary>
    /// Adds support for KubeCtl to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add support for KubeCtl to.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddKubeCtlSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IKubeCtlService, KubeCtlService>();

    /// <summary>
    /// Adds the shell execution service to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the shell execution service to.</param>
    /// <returns>The modified service collection.</returns>
    private static IServiceCollection AddShellExecution(this IServiceCollection services) =>
        services.AddSingleton<IShellExecutionService, ShellExecutionService>();

    /// <summary>
    /// Adds container support to the IServiceCollection. </summary> <param name="services">The IServiceCollection to add the container support to.</param> <returns>The modified IServiceCollection with the added container support.</returns>
    /// /
    private static IServiceCollection AddContainerSupport(this IServiceCollection services) =>
        services
            .AddSingleton<IProjectPropertyService, ProjectPropertyService>()
            .AddSingleton<IContainerCompositionService, ContainerCompositionService>()
            .AddSingleton<IContainerDetailsService, ContainerDetailsService>();
}
