using Aspirate.Shared.Interfaces.Commands;

namespace Aspirate.Commands.Options;

public sealed class NonInteractiveOption : BaseOption<bool>
{
    private static readonly string[] _aliases =
    {
        "--non-interactive"
    };

    private NonInteractiveOption() : base(_aliases, "ASPIRATE_NON_INTERACTIVE", false)
    {
        Name = nameof(ICommandOptions.NonInteractive);
        Description = "Disables interactive mode for the command";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static NonInteractiveOption Instance { get; } = new();
}
