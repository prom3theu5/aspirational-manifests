using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class ComposeBuildsOption : BaseOption<List<string>?>
{
    private static readonly string[] _aliases = ["--compose-build"];

    private ComposeBuildsOption() : base(_aliases, "ASPIRATE_COMPOSE_BUILDS", null)
    {
        Name = nameof(IBuildOptions.ComposeBuilds);
        Description = "Specify the resource names which will be built by the compose file.";
        Arity = ArgumentArity.ZeroOrMore;
        IsRequired = false;
    }

    public static ComposeBuildsOption Instance { get; } = new();
}
