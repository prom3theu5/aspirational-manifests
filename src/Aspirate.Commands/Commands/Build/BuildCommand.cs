namespace Aspirate.Commands.Commands.Build;

public sealed class BuildCommand : BaseCommand<BuildOptions, BuildCommandHandler>
{
    protected override bool CommandUnlocksSecrets => false;

    public BuildCommand() : base("build", "Builds and pushes containers")
    {
       AddOption(ProjectPathOption.Instance);
       AddOption(AspireManifestOption.Instance);
       AddOption(ContainerBuilderOption.Instance);
       AddOption(ContainerBuildContextOption.Instance);
       AddOption(ContainerImageTagOption.Instance);
       AddOption(ContainerBuildArgsOption.Instance);
       AddOption(PreferDockerfileOption.Instance);
       AddOption(ContainerRegistryOption.Instance);
       AddOption(ContainerRepositoryPrefixOption.Instance);
       AddOption(RuntimeIdentifierOption.Instance);
       AddOption(ComponentsOption.Instance);
    }
}
