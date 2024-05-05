namespace Aspirate.Commands.Commands.Destroy;

public sealed class DestroyCommand : BaseCommand<DestroyOptions, DestroyCommandHandler>
{
    protected override bool CommandUnlocksSecrets => false;

    public DestroyCommand() : base("destroy", "Removes the manifests from your cluster..")
    {
        AddOption(InputPathOption.Instance);
        AddOption(KubernetesContextOption.Instance);
    }
}
