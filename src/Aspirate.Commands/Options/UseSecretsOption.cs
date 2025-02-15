namespace Aspirate.Commands.Options;

public sealed class UseSecretsOption : BaseOption<bool?>
{
    private static readonly string[] _aliases = ["--use-secrets"];

    private UseSecretsOption() : base(_aliases, "ASPIRATE_USE_SECRETS", false)
    {
        Name = nameof(IBuildOptions.UseSecrets);
        Description = "Include secrets as part of the build process";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static UseSecretsOption Instance { get; } = new();
}
