using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class TemplatePathOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "-tp",
        "--template-path"
    };

    private TemplatePathOption() : base(_aliases, "ASPIRATE_TEMPLATE_PATH", null)
    {
        Name = nameof(IInitOptions.TemplatePath);
        Description = "The Custom Template path to use";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static TemplatePathOption Instance { get; } = new();
}
