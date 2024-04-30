namespace Aspirate.Commands.Options;

public sealed class ComposeBuildsOption : BaseOption<bool?>
{
    private static readonly string[] _aliases = ["--compose-builds"];

    private ComposeBuildsOption() : base(_aliases, "ASPIRATE_COMPOSE_BUILDS", null)
    {
        Name = nameof(IBuildOptions.ComposeBuilds);
        Description = "Compose builds for the container image.";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static ComposeBuildsOption Instance { get; } = new();
}
