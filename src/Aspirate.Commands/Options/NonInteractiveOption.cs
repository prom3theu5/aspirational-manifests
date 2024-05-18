namespace Aspirate.Commands.Options;

public sealed class NonInteractiveOption : BaseOption<bool?>
{
    private static readonly string[] _aliases =
    [
        "--non-interactive"
    ];

    private NonInteractiveOption() : base(_aliases, "ASPIRATE_NON_INTERACTIVE", null)
    {
        Name = nameof(ICommandOptions.NonInteractive);
        Description = "Disables interactive mode for the command";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static NonInteractiveOption Instance { get; } = new();
}
