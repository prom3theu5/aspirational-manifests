namespace Aspirate.Commands.Options;
public sealed class EnableMinikubeMountActionOption : BaseOption<bool?>
{
    private static readonly string[] _aliases =
    [
        "-em",
        "--enable-minikube-mount"
    ];

    private EnableMinikubeMountActionOption() : base(_aliases, "ASPIRATE_ENABLE_MINIKUBE_MOUNT_ACTION", null)
    {
        Name = nameof(IMinikubeOptions.EnableMinikubeMountAction);
        Description = "Enables minikube mount actions";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static EnableMinikubeMountActionOption Instance { get; } = new();

    public override bool IsSecret => false;
}
