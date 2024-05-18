namespace Aspirate.Commands.Options
{
    public sealed class ReplaceSecretsOption : BaseOption<bool?>
    {
        private static readonly string[] _aliases = ["--replace-secrets"];

        private ReplaceSecretsOption() : base(_aliases, "ASPIRATE_REPLACE_SECRETS", null)
        {
            Name = nameof(ISecretState.ReplaceSecrets);
            Description = "Replace all secrets and inputs.";
            Arity = ArgumentArity.ZeroOrOne;
            IsRequired = false;
        }

        public static ReplaceSecretsOption Instance { get; } = new();
    }
}
