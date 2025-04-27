namespace Aspirate.Processors.Resources.AbstractProcessors;

/// <summary>
/// A container component for version 0 of Aspire.
/// </summary>
public class ContainerProcessor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService,
    IManifestWriter manifestWriter)
        : BaseContainerProcessor<ContainerResource>(
            fileSystem,
            console,
            secretProvider,
            containerCompositionService,
            containerDetailsService,
            manifestWriter)
{
    /// <inheritdoc />
    public override string ResourceType => AspireComponentLiterals.Container;
}
