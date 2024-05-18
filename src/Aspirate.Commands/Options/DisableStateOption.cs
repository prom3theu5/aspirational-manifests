namespace Aspirate.Commands.Options
{
    public sealed class DisableStateOption : BaseOption<bool?>
    {
        private static readonly string[] _aliases = ["--disable-state", "--no-state"];

        private DisableStateOption() : base(_aliases, "ASPIRATE_DISABLE_STATE", null)
        {
            Name = nameof(ICommandOptions.DisableState);
            Description = "Disables State Support";
            Arity = ArgumentArity.ZeroOrOne;
            IsRequired = false;
        }

        public static DisableStateOption Instance { get; } = new();
    }
}
