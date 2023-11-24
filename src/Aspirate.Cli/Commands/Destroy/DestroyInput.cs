namespace Aspirate.Cli.Commands.Destroy;

public sealed class DestroyInput : CommandSettings
{
    /// <summary>
    /// The path to the input kustomize manifest
    /// </summary>
    [CommandOption("-k|--kustomize")]
    [Description("The input path for the kustomize manifests")]
    public string OutputPathFlag { get; init; } = AspirateLiterals.DefaultOutputPath;
}
