namespace Aspirate.Commands.Options;

public sealed class PrivateRegistryUsernameOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--private-registry-username",
    };

    private PrivateRegistryUsernameOption() : base(_aliases, "ASPIRATE_PRIVATE_REGISTRY_USERNAME", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.PrivateRegistryUsername);
        Description = "The Private Registry username.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static PrivateRegistryUsernameOption Instance { get; } = new();
}
