namespace Aspirate.Commands.Options;

public sealed class OutputPathOption : BaseOption<string>
{
    private static readonly string[] _aliases =
    {
        "-o",
        "--output-path",
    };

    private OutputPathOption() : base(_aliases,  "ASPIRATE_OUTPUT_PATH", AspirateLiterals.DefaultOutputPath)
    {
        Name = nameof(IGenerateOptions.OutputPath);
        Description = "The output path for generated manifests";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static OutputPathOption Instance { get; } = new();
}
