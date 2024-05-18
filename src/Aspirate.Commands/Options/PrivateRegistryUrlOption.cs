namespace Aspirate.Commands.Options;

public sealed class PrivateRegistryUrlOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    [
        "--private-registry-url"
    ];

    private PrivateRegistryUrlOption() : base(_aliases, "ASPIRATE_PRIVATE_REGISTRY_URL", null)
    {
        Name = nameof(IPrivateRegistryCredentialsOptions.PrivateRegistryUrl);
        Description = "The Private Registry url.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static PrivateRegistryUrlOption Instance { get; } = new();
}
