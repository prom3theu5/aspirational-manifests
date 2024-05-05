namespace Aspirate.Commands.Options;

public sealed class InputPathOption : BaseOption<string>
{
    private static readonly string[] _aliases =
    {
        "-i",
        "--input-path"
    };

    private InputPathOption() : base(_aliases, "ASPIRATE_INPUT_PATH", AspirateLiterals.DefaultArtifactsPath)
    {
        Name = nameof(IKubernetesOptions.InputPath);
        Description = "The path for the kustomize manifests directory";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static InputPathOption Instance { get; } = new();
}
