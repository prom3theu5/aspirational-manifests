namespace Aspirate.Commands.Commands.Run;

public sealed class RunCommand : BaseCommand<RunOptions, RunCommandHandler>
{
    protected override bool CommandUnlocksSecrets => true;

    public RunCommand() : base("run", "Builds, pushes containers, and runs the current solution directly against a kubernetes cluster.")
    {
       AddOption(ProjectPathOption.Instance);
       AddOption(AspireManifestOption.Instance);
       AddOption(SkipBuildOption.Instance);
       AddOption(ContainerBuilderOption.Instance);
       AddOption(ContainerBuildContextOption.Instance);
       AddOption(ContainerImageTagOption.Instance);
       AddOption(ContainerBuildArgsOption.Instance);
       AddOption(PreferDockerfileOption.Instance);
       AddOption(ContainerRegistryOption.Instance);
       AddOption(ContainerRepositoryPrefixOption.Instance);
       AddOption(ImagePullPolicyOption.Instance);
       AddOption(NamespaceOption.Instance);
       AddOption(RuntimeIdentifierOption.Instance);
       AddOption(SecretPasswordOption.Instance);
       AddOption(PrivateRegistryOption.Instance);
       AddOption(PrivateRegistryUrlOption.Instance);
       AddOption(PrivateRegistryUsernameOption.Instance);
       AddOption(PrivateRegistryPasswordOption.Instance);
       AddOption(PrivateRegistryEmailOption.Instance);
       AddOption(IncludeDashboardOption.Instance);
       AddOption(AllowClearNamespaceOption.Instance);
    }
}
