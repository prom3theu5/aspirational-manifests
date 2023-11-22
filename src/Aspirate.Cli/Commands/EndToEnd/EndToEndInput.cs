namespace Aspirate.Cli.Commands.EndToEnd;

/// <summary>
/// The input for the EndToEndCommand
/// </summary>
public sealed class EndToEndInput : CommandSettings
{
    /// <summary>
    /// The path to the Aspire manifest
    /// </summary>
    [CommandOption("-p|--project")]
    [Description("The path to the Aspire AppHost project")]
    public required string PathToAspireProjectFlag { get; init; }

    /// <summary>
    /// The path to the output kustomize manifest
    /// </summary>
    [CommandOption("-o|--output")]
    [Description("The output path for the generated kustomize manifests")]
    public required string OutputPathFlag { get; init; }
}
