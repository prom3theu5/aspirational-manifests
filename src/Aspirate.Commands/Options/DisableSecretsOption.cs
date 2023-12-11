namespace Aspirate.Commands.Options
{
    public sealed class DisableSecretsOption : BaseOption<bool>
    {
        private static readonly string[] _aliases = { "--disable-secrets" };

        private DisableSecretsOption() : base(_aliases, "ASPIRATE_DISABLE_SECRETS", false)
        {
            Name = nameof(BaseCommandOptions.DisableSecrets);
            Description = "Disables Secret Support";
            Arity = ArgumentArity.ZeroOrOne;
            IsRequired = false;
        }

        public static DisableSecretsOption Instance { get; } = new();
    }
}
