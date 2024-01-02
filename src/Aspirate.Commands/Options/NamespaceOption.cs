namespace Aspirate.Commands.Options;

public sealed class NamespaceOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--namespace",
    };

    private NamespaceOption() : base(_aliases,  "ASPIRATE_NAMESPACE", string.Empty)
    {
        Name = nameof(IGenerateOptions.Namespace);
        Description = "The Namespace to use for deployments";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static NamespaceOption Instance { get; } = new();
}
