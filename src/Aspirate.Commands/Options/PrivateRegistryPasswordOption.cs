using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class PrivateRegistryPasswordOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--private-registry-password",
    };

    private PrivateRegistryPasswordOption() : base(_aliases, "ASPIRATE_PRIVATE_REGISTRY_PASSWORD", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.PrivateRegistryPassword);
        Description = "The Private Registry password.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static PrivateRegistryPasswordOption Instance { get; } = new();
}
