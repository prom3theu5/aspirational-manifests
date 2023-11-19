namespace Aspirate.Commands.EndToEnd;

/// <summary>
/// The input for the EndToEndCommand
/// </summary>
public sealed class EndToEndInput : CommandSettings
{
    /// <summary>
    /// The path to the Aspire manifest
    /// </summary>
    [CommandOption("-m|--manifest")]
    [Description("The path to the Aspire manifest")]
    public required string PathToAspireManifestFlag { get; init; }

    /// <summary>
    /// The path to the output kustomize manifest
    /// </summary>
    [CommandOption("-o|--output")]
    [Description("The output path for the generated kustomize manifests")]
    public required string OutputPathFlag { get; init; }
}
