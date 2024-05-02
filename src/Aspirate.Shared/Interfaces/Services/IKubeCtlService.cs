namespace Aspirate.Shared.Interfaces.Services;

/// <summary>
/// Represents a service for interacting with Kubernetes using `kubectl` commands.
/// </summary>
public interface IKubeCtlService
{
    /// <summary>
    /// Selects the Kubernetes context for deployment.
    /// </summary>
    /// <returns>
    /// The selected Kubernetes context for deployment, or null if no context was found.
    /// </returns>
    Task<string?> SelectKubernetesContextForDeployment();

    /// <summary>
    /// Applies the manifests to the specified context and output folder.
    /// </summary>
    /// <param name="context">The context to which the manifests should be applied.</param>
    /// <param name="outputFolder">The output folder where the applied manifests should be saved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the manifests were successfully applied.</returns>
    /// <remarks>
    /// The ApplyManifests method applies the manifests to the specified context and saves the applied manifests
    /// to the output folder. The method returns a boolean indicating whether the operation was successful.
    /// </remarks>
    Task<bool> ApplyManifests(string context, string outputFolder);

    /// <summary>
    /// Performs a rolling restart of the Kubernetes resources in the specified context and output folder.
    /// </summary>
    /// <param name="context">The context in which the rolling restart should be performed.</param>
    /// <param name="outputFolder">The output folder that contains the manifests of the resources.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.
    /// The task result indicates whether the rolling restart was successful.</returns>
    Task<bool> PerformRollingRestart(string context, string outputFolder);

    /// <summary>
    /// Removes manifests from the specified context and output folder.
    /// </summary>
    /// <param name="context">The context from which to remove manifests.</param>
    /// <param name="outputFolder">The output folder in which to remove manifests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.
    /// The task result represents whether the manifests were successfully removed.</returns>
    Task<bool> RemoveManifests(string context, string outputFolder);
}
