namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyInput : CommandSettings
{
    /// <summary>
    /// The path to the output kustomize manifest
    /// </summary>
    [CommandOption("-k|--kustomize")]
    [Description("The input path for the generated kustomize manifests")]
    public string OutputPathFlag { get; init; } = AspirateLiterals.DefaultOutputPath;
}
