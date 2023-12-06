namespace Aspirate.Commands.Commands.Destroy;

public sealed class DestroyCommand : BaseCommand<DestroyOptions, DestroyCommandHandler>
{
    public DestroyCommand() : base("destroy", "Removes the manifests from your cluster..")
    {
        AddOption(SharedOptions.ManifestDirectoryPath);
        AddOption(SharedOptions.KubernetesContext);
    }
}
