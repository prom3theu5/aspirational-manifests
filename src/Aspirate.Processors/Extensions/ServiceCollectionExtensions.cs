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
    public static void AddAspirateProcessors(this IServiceCollection services) =>
        services
            .RegisterProcessor<PostgresServerProcessor>(AspireLiterals.PostgresServer)
            .RegisterProcessor<PostgresDatabaseProcessor>(AspireLiterals.PostgresDatabase)
            .RegisterProcessor<ProjectProcessor>(AspireLiterals.Project)
            .RegisterProcessor<DockerfileProcessor>(AspireLiterals.Dockerfile)
            .RegisterProcessor<RedisProcessor>(AspireLiterals.Redis)
            .RegisterProcessor<RabbitMqProcessor>(AspireLiterals.RabbitMq)
            .RegisterProcessor<ContainerProcessor>(AspireLiterals.Container)
            .RegisterProcessor<FinalProcessor>(AspireLiterals.Final);

    /// <summary>
    /// Registers a processor implementation with a specified key in the service collection.
    /// </summary>
    /// <typeparam name="TImplementation">The type of the processor implementation to register.</typeparam>
    /// <param name="services">The service collection to register the processor implementation with.</param>
    /// <param name="key">The key associated with the processor implementation.</param>
    /// <returns>The updated service collection with the processor implementation registered.</returns>
    private static IServiceCollection RegisterProcessor<TImplementation>(this IServiceCollection services, string key) where TImplementation : class, IProcessor =>
        services.AddKeyedSingleton<IProcessor, TImplementation>(key);
}
