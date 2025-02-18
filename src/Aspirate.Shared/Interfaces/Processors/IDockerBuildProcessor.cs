namespace Aspirate.Shared.Interfaces.Processors;

public interface IDockerBuildProcessor
{
    public Task BuildAndPushContainerForDockerfile(KeyValuePair<string, Resource> resource, ContainerOptions options,
        bool nonInteractive);

    public void PopulateContainerImageCacheWithImage(KeyValuePair<string, Resource> resource, ContainerOptions options);
}
