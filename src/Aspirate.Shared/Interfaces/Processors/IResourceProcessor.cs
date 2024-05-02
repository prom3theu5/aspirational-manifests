namespace Aspirate.Shared.Interfaces.Processors;

public interface IResourceProcessor
{
    /// <summary>
    /// The resource type that this handler is for.
    /// </summary>
    string ResourceType { get; }

    /// <summary>
    /// Serializes the resource to JSON.
    /// </summary>
    /// <param name="reader">The reader instance.</param>
    /// <returns>A Resource instance.</returns>
    Resource? Deserialize(ref Utf8JsonReader reader);

    /// <summary>
    /// CreateManifests method is used to create manifests for a specific resource in the Aspirate application.
    /// It takes in a CreateManifestsOptions object as a parameter that contains information about the resource, output path, and image pull policy.
    /// This method is overridden in the ProjectProcessor class to provide implementation specific to the Project resource.
    /// </summary>
    /// <param name="options">The options for creating manifests.</param>
    /// <returns>A Task object representing the asynchronous operation. The task completes when the manifests are created.</returns>
    Task<bool> CreateManifests(CreateManifestsOptions options);

    /// <summary>
    /// Creates a compose entry with the given resource.
    /// </summary>
    /// <param name="options">The create compose entry options instance.</param>
    /// <returns>The created compose entry service, or null if creation is not overridden.</returns>
    ComposeService CreateComposeEntry(CreateComposeEntryOptions options);

    /// <summary>
    /// Creates Kubernetes objects for a specific resource in the Aspirate application.
    /// </summary>
    /// <param name="options">Options for creating Kubernetes objects.</param>
    /// <returns>An array of objects representing the created Kubernetes objects.</returns>
    object[] CreateKubernetesObjects(CreateKubernetesObjectsOptions options);
}
