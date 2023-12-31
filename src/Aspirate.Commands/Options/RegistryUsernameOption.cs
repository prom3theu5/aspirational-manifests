namespace Aspirate.Commands.Options;

public sealed class RegistryUsernameOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--registry-username",
    };

    private RegistryUsernameOption() : base(_aliases, "ASPIRATE_REGISTRY_USERNAME", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.RegistryUsername);
        Description = "The Private Registry username.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static RegistryUsernameOption Instance { get; } = new();
}
