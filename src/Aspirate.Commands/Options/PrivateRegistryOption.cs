namespace Aspirate.Commands.Options;

public sealed class PrivateRegistryOption : BaseOption<bool?>
{
    private static readonly string[] _aliases =
    [
        "--private-registry"
    ];

    private PrivateRegistryOption() : base(_aliases, "ASPIRATE_PRIVATE_REGISTRY", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.WithPrivateRegistry);
        Description = "Enables Private registry imagePullSecret. You will need to supply username and password as well.";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static PrivateRegistryOption Instance { get; } = new();

    public override bool IsSecret => false;
}
