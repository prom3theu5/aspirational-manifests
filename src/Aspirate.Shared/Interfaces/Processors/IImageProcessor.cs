namespace Aspirate.Shared.Interfaces.Processors;

public interface IImageProcessor
{
    /// <summary>
    /// Builds and pushes a dockerfile.
    /// </summary>
    /// <param name="resource">The resource containing information about the docker file.</param>
    /// <param name="options">The container options.</param>
    /// <param name="nonInteractive">Flag indicating whether the process should run in non-interactive mode.</param>
    /// <returns>A task representing an asynchronous operation.</returns>
    Task BuildAndPushContainerForDockerfile(
        KeyValuePair<string, Resource> resource,
        ContainerOptions options,
        bool nonInteractive);

    /// <summary>
    /// Populates a container cache with names generated from options.
    /// </summary>
    /// <param name="resource">The resource associated with the cached image.</param>
    /// <param name="options">The options used to generate image names.</param>
    void PopulateContainerImageCacheWithImage(
        KeyValuePair<string, Resource> resource,
        ContainerOptions options);
}
