namespace Aspirate.Services.Interfaces;

/// <summary>
/// Represents a service for retrieving container details.
/// </summary>
public interface IContainerDetailsService
{
    /// <summary>
    /// Gets the details of a container.
    /// </summary>
    /// <param name="resourceName">The name of the resource.</param>
    /// <param name="projectResource">The project instance.</param>
    /// <param name="containerRegistry">The optional container registry.</param>
    /// <param name="containerImageTag">The optional container image tag.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="MsBuildContainerProperties"/> object.</returns>
    Task<MsBuildContainerProperties> GetContainerDetails(string resourceName, ProjectResource projectResource, string? containerRegistry, string? containerImageTag);
}
