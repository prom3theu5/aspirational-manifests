namespace Aspirate.Commands.Options;

public sealed class SecretProviderOption : BaseOption<ProviderType>
{
    private static readonly string[] _aliases =
    {
        "--secret-provider"
    };

    private SecretProviderOption() : base(_aliases, "ASPIRATE_SECRET_PROVIDER", ProviderType.Password)
    {
        Name = nameof(BaseCommandOptions.SecretProvider);
        Description = "Sets the secret provider. Default is 'Password'";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static SecretProviderOption Instance { get; } = new();
}
