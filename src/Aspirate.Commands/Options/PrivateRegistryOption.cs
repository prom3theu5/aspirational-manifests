using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class PrivateRegistryOption : BaseOption<bool?>
{
    private static readonly string[] _aliases =
    {
        "--private-registry",
    };

    private PrivateRegistryOption() : base(_aliases, "ASPIRATE_PRIVATE_REGISTRY", false)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.WithPrivateRegistry);
        Description = "Enables Private registry imagePullSecret. You will need to supply username and password as well.";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static PrivateRegistryOption Instance { get; } = new();
}
