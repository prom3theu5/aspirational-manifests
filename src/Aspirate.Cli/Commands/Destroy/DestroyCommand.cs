namespace Aspirate.Cli.Commands.Destroy;

public sealed class DestroyCommand : BaseCommand<DestroyOptions, DestroyCommandHandler>
{
    public DestroyCommand() : base("destroy", "Removes the manifests from your cluster..") =>
        AddOption(new Option<string>(new[] { "-i", "--input-path" })
        {
            Description = "The input path for the kustomize manifests to remove from the cluster",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });
}
