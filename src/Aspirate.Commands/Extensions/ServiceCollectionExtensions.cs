namespace Aspirate.Commands.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register services for AspirateState and AspirateActions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AspirateState service to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    /// <remarks>
    /// This method adds the AspirateState service as a singleton to the IServiceCollection.
    /// </remarks>
    public static IServiceCollection AddAspirateState(this IServiceCollection services) =>
        services.AddSingleton<AspirateState>();

    /// <summary>
    /// Registers a series of actions for the Aspirate application with the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the actions with.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> after registering the actions.</returns>
    public static IServiceCollection AddAspirateActions(this IServiceCollection services) =>
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
            .RegisterAction<RemoveManifestsFromClusterAction>()
            .RegisterAction<SubstituteValuesAspireManifestAction>()
            .RegisterAction<PopulateInputsAction>()
            .RegisterAction<SaveInputsAction>();

    /// <summary>
    /// Registers an implementation of <see cref="IAction"/> with the specified <typeparamref name="TImplementation"/> type.
    /// </summary>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the implementation with.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection RegisterAction<TImplementation>(this IServiceCollection services) where TImplementation : class, IAction =>
        services.AddKeyedSingleton<IAction, TImplementation>(typeof(TImplementation).Name);
}
