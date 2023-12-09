namespace Aspirate.Services.Interfaces;

/// <summary>
/// Represents a service for container composition.
/// </summary>
public interface IContainerCompositionService
{
    /// <summary>
    /// Builds and pushes a container for the specified project using the specified container details and interactive mode flag.
    /// </summary>
    /// <param name="project">The project to build and push the container for.</param>
    /// <param name="containerDetails">The container properties used to build and push the container.</param>
    /// <param name="nonInteractive">Flag indicating whether the process should run in non-interactive mode.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result will be true if the container build and push was successful,
    /// or false if there was an error during the process.
    /// </returns>
    Task<bool> BuildAndPushContainerForProject(Project project, MsBuildContainerProperties containerDetails, bool nonInteractive);

    /// <summary>
    /// Build and push a container for a Dockerfile.
    /// </summary>
    /// <param name="dockerfile">The Dockerfile to build the container from.</param>
    /// <param name="builder">The builder to use.</param>
    /// <param name="imageName">The name of the image to create.</param>
    /// <param name="registry">The registry to push the image to.</param>
    /// <param name="nonInteractive">Flag to determine if the build process should be non-interactive.</param>
    /// <returns>
    /// A <see cref="Task{T}"/> representing the asynchronous operation.
    /// The task will return true if the container was built and pushed successfully,
    /// otherwise it will return false.
    /// </returns>
    /// <remarks>
    /// This method builds a container using the provided Dockerfile and builder.
    /// It then pushes the created image to the specified registry.
    /// The nonInteractive parameter can be set to true to suppress any interactive prompts during the build process.
    /// </remarks>
    Task<bool> BuildAndPushContainerForDockerfile(Dockerfile dockerfile, string builder, string imageName, string? registry, bool nonInteractive);
}
