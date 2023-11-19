namespace Aspirational.Manifests.Commands.EndToEnd;

/// <summary>
/// The input for the EndToEndCommand
/// </summary>
public sealed class EndToEndInput
{
    /// <summary>
    /// The path to the Aspire manifest
    /// </summary>
    [FlagAlias("manifest",'m')]
    [Description("The path to the Aspire manifest", Name = "Manifest Path")]
    public required string PathToAspireManifestFlag { get; init; }

    /// <summary>
    /// The path to the output kustomize manifest
    /// </summary>
    [FlagAlias("output",'o')]
    [Description("The output path for the generated kustomize manifests", Name = "Output Path")]
    public required string OutputPathFlag { get; init; }
}