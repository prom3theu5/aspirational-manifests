namespace Aspirate.Processors.Extensions;

/// <summary>
/// Provides extension methods for IServiceCollection to register processors for the Aspirate process.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the necessary Aspirate processors to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the processors to.</param>
    public static IServiceCollection AddAspirateProcessors(this IServiceCollection services) =>
        services
            .RegisterProcessor<ProjectProcessor>(AspireComponentLiterals.Project)
            .RegisterProcessor<DockerfileProcessor>(AspireComponentLiterals.Dockerfile)
            .RegisterProcessor<ContainerProcessor>(AspireComponentLiterals.Container)
            .RegisterProcessor<DaprProcessor>(AspireComponentLiterals.DaprSystem)
            .RegisterProcessor<DaprComponentProcessor>(AspireComponentLiterals.DaprComponent)
            .RegisterProcessor<ParameterProcessor>(AspireComponentLiterals.Parameter)
            .RegisterProcessor<ValueProcessor>(AspireComponentLiterals.Value)
            .RegisterProcessor<FinalProcessor>(AspireLiterals.Final);

    /// <summary>
    /// Adds the necessary Aspirate processor Value Substitutors to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the processors to.</param>
    public static IServiceCollection AddAspiratePlaceholderSubstitutionStrategies(this IServiceCollection services) =>
        services
            .AddSingleton<IPlaceholderSubstitutionStrategy, ResourceBindingsSubstitutionStrategy>()
            .AddSingleton<IPlaceholderSubstitutionStrategy, ResourceConnectionStringSubstitutionStrategy>()
            .AddSingleton<IPlaceholderSubstitutionStrategy, ResourceParameterSubstitutionStrategy>()
            .AddSingleton<IPlaceholderSubstitutionStrategy, ResourceValueSubstitutionStrategy>();

    /// <summary>
    /// Registers a processor implementation with a specified key in the service collection.
    /// </summary>
    /// <typeparam name="TImplementation">The type of the processor implementation to register.</typeparam>
    /// <param name="services">The service collection to register the processor implementation with.</param>
    /// <param name="key">The key associated with the processor implementation.</param>
    /// <returns>The updated service collection with the processor implementation registered.</returns>
    private static IServiceCollection RegisterProcessor<TImplementation>(this IServiceCollection services, string key) where TImplementation : class, IResourceProcessor =>
        services.AddKeyedSingleton<IResourceProcessor, TImplementation>(key);
}
