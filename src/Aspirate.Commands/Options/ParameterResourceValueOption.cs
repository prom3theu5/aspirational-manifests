namespace Aspirate.Commands.Options;

public sealed class ParameterResourceValueOption : BaseOption<List<string>?>
{
    private static readonly string[] _aliases =
    [
        "-pa",
        "--parameter"
    ];

    private ParameterResourceValueOption() : base(_aliases, "ASPIRATE_PARAMETER_VALUE", null)
    {
        Name = nameof(IGenerateOptions.Parameters);
        Description = "The parameter resource value.";
        Arity = ArgumentArity.ZeroOrMore;
        IsRequired = false;
    }

    public static ParameterResourceValueOption Instance { get; } = new();
}
