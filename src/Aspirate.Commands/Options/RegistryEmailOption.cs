namespace Aspirate.Commands.Options;

public sealed class RegistryEmailOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--registry-email",
    };

    private RegistryEmailOption() : base(_aliases, "ASPIRATE_REGISTRY_EMAIL", "aspirate@aspirate.com")
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.RegistryEmail);
        Description = "The Private Registry email. It is required and defaults to 'aspirate@aspirate.com'.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static RegistryEmailOption Instance { get; } = new();
}
