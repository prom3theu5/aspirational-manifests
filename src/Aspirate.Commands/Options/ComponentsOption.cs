namespace Aspirate.Commands.Options;

public sealed class ComponentsOption : BaseOption<List<string>?>
{
    private static readonly string[] _aliases =
    [
        "-c",
        "--components"
    ];

    private ComponentsOption() : base(_aliases, "ASPIRATE_COMPONENTS", null)
    {
        Name = nameof(IComponentsOptions.CliSpecifiedComponents);
        Description = "Specify which components build or generate, non interactively";
        Arity = ArgumentArity.ZeroOrMore;
        IsRequired = false;
    }

    public static ComponentsOption Instance { get; } = new();

    public override bool IsSecret => false;
}
