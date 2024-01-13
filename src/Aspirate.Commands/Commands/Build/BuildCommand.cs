namespace Aspirate.Commands.Commands.Build;

public sealed class BuildCommand : BaseCommand<BuildOptions, BuildCommandHandler>
{
    public BuildCommand() : base("build", "Builds and pushes containers")
    {
       AddOption(ProjectPathOption.Instance);
       AddOption(AspireManifestOption.Instance);
       AddOption(ContainerBuilderOption.Instance);
       AddOption(ContainerImageTagOption.Instance);
       AddOption(ContainerRegistryOption.Instance);
       AddOption(ContainerRepositoryPrefixOption.Instance);
       AddOption(RuntimeIdentifierOption.Instance);
    }
}
