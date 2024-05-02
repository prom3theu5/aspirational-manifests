using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class SkipHelmGenerationOption : BaseOption<bool>
{
    private static readonly string[] _aliases =
    {
        "-sh",
        "--skip-hem",
        "--skip-helm-generation"
    };

    private SkipHelmGenerationOption() : base(_aliases, "ASPIRATE_SKIP_HELM_GENERATION", false)
    {
        Name = nameof(IGenerateOptions.SkipHelmGeneration);
        Description = "Skips generation of a helm chart";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static SkipHelmGenerationOption Instance { get; } = new();
}
