using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class SecretPasswordOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--secret-password"
    };

    private SecretPasswordOption() : base(_aliases, "ASPIRATE_SECRET_PASSWORD", null)
    {
        Name = nameof(ISecretOptions.SecretPassword);
        Description = "The Secret Password to use";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static SecretPasswordOption Instance { get; } = new();
}
