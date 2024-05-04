namespace Aspirate.Commands.Options;

public sealed class SecretProviderOption : BaseOption<SecretProviderType>
{
    private static readonly string[] _aliases =
    {
        "--secret-provider"
    };

    private SecretProviderOption() : base(_aliases, "ASPIRATE_SECRET_PROVIDER", SecretProviderType.Password)
    {
        Name = nameof(ICommandOptions.SecretProvider);
        Description = "Sets the secret provider. Default is 'Password'";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static SecretProviderOption Instance { get; } = new();
}
