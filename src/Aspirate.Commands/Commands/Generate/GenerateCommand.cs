namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateCommand : BaseCommand<GenerateOptions, GenerateCommandHandler>
{
    public GenerateCommand() : base("generate", "Builds, pushes containers, generates aspire manifest and kustomize manifests.")
    {
       AddOption(ProjectPathOption.Instance);
       AddOption(AspireManifestOption.Instance);
       AddOption(OutputPathOption.Instance);
       AddOption(SkipBuildOption.Instance);
       AddOption(SkipFinalKustomizeGenerationOption.Instance);
       AddOption(ContainerBuilderOption.Instance);
       AddOption(ContainerImageTagOption.Instance);
       AddOption(ContainerRegistryOption.Instance);
       AddOption(ContainerRepositoryPrefixOption.Instance);
       AddOption(ImagePullPolicyOption.Instance);
       AddOption(NamespaceOption.Instance);
       AddOption(OutputFormatOption.Instance);
       AddOption(RuntimeIdentifierOption.Instance);
       AddOption(SecretPasswordOption.Instance);
       AddOption(PrivateRegistryOption.Instance);
       AddOption(PrivateRegistryUrlOption.Instance);
       AddOption(PrivateRegistryUsernameOption.Instance);
       AddOption(PrivateRegistryPasswordOption.Instance);
       AddOption(PrivateRegistryEmailOption.Instance);
       AddOption(IncludeDashboardOption.Instance);
    }
}
