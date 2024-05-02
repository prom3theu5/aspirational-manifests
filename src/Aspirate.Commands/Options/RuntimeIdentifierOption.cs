using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class RuntimeIdentifierOption : BaseOption<string?>
{
    private static readonly string[] _aliases = ["--runtime-identifier"];

    private RuntimeIdentifierOption() : base(_aliases, "ASPIRATE_RUNTIME_IDENTIFIER", null)
    {
        Name = nameof(IBuildOptions.RuntimeIdentifier);
        Description = "The Custom Runtime identifier to use for .net project builds.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static RuntimeIdentifierOption Instance { get; } = new();
}
