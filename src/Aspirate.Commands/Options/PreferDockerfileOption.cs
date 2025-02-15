namespace Aspirate.Commands.Options;

public sealed class PreferDockerfileOption : BaseOption<bool?>
{
    private static readonly string[] _aliases = ["--prefer-dockerfile"];

    private PreferDockerfileOption() : base(_aliases, "ASPIRATE_PREFER_DOCKERFILE", null)
    {
        Name = nameof(IBuildOptions.PreferDockerfile);
        Description = "Instructs to use Dockerfile when available to build project images";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static PreferDockerfileOption Instance { get; } = new();
}
