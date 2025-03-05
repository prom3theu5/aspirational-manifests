namespace Aspirate.Shared.Interfaces.Processors;

public interface IImageProcessor
{
    Task BuildAndPushContainerForDockerfile(
        KeyValuePair<string, Resource> resource,
        ContainerOptions options,
        bool nonInteractive);

    void PopulateContainerImageCacheWithImage(
        KeyValuePair<string, Resource> resource,
        ContainerOptions options);
}
