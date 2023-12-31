namespace Aspirate.Commands.Options;

public sealed class RegistryPasswordOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--registry-password",
    };

    private RegistryPasswordOption() : base(_aliases, "ASPIRATE_REGISTRY_PASSWORD", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.RegistryPassword);
        Description = "The Private Registry password.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static RegistryPasswordOption Instance { get; } = new();
}
