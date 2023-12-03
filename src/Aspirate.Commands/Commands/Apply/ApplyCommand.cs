namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyCommand : BaseCommand<ApplyOptions, ApplyCommandHandler>
{
    public ApplyCommand() : base("apply", "Apply the generated kustomize manifest to the cluster.")
    {
        AddOption(SharedOptions.ManifestDirectoryPath);
        AddOption(SharedOptions.KubernetesContext);
    }
}
