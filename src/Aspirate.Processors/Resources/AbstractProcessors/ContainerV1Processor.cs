namespace Aspirate.Processors.Resources.AbstractProcessors;

/// <summary>
/// A container component for version 1 of Aspire.
/// </summary>
public class ContainerV1Processor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService,
    IManifestWriter manifestWriter)
        : BaseContainerProcessor<ContainerV1Resource>(
            fileSystem,
            console,
            secretProvider,
            containerCompositionService,
            containerDetailsService,
            manifestWriter)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.ContainerV1;
}
