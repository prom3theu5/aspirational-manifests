namespace Aspirate.Commands.Options;

public sealed class SkipBuildOption : BaseOption<bool?>
{
    private static readonly string[] _aliases = ["--skip-build"];

    private SkipBuildOption() : base(_aliases, "ASPIRATE_SKIP_BUILD", null)
    {
        Name = nameof(IGenerateOptions.SkipBuild);
        Description = "Skips build and Push of containers";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static SkipBuildOption Instance { get; } = new();
}
