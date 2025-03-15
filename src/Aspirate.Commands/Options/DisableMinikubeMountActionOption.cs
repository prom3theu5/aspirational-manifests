namespace Aspirate.Commands.Options;
public sealed class DisableMinikubeMountActionOption : BaseOption<bool?>
{
    private static readonly string[] _aliases =
    [
        "-dm",
        "--disable-minikube-mount"
    ];

    private DisableMinikubeMountActionOption() : base(_aliases, "ASPIRATE_DISABLE_MINIKUBE_MOUNT_ACTION", null)
    {
        Name = nameof(IMinikubeOptions.DisableMinikubeMountAction);
        Description = "Disables minikube mount actions";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static DisableMinikubeMountActionOption Instance { get; } = new();

    public override bool IsSecret => false;
}
