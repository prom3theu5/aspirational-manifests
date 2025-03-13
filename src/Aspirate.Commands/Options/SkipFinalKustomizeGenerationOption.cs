namespace Aspirate.Commands.Options;

public sealed class SkipFinalKustomizeGenerationOption : BaseOption<bool?>
{
    private static readonly string[] _aliases =
    [
        "-sf",
        "--skip-final",
        "--skip-final-kustomize-generation"
    ];

    private SkipFinalKustomizeGenerationOption() : base(_aliases, "ASPIRATE_SKIP_FINAL_KUSTOMIZE_GENERATION", null)
    {
        Name = nameof(IGenerateOptions.SkipFinalKustomizeGeneration);
        Description = "Skips The final generation of the kustomize manifest, which is the parent top level file";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static SkipFinalKustomizeGenerationOption Instance { get; } = new();

    public override bool IsSecret => false;
}
