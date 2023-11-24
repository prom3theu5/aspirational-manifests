namespace Aspirate.Cli.Commands.Generate;

public sealed class GenerateInput : CommandSettings
{
    /// <summary>
    /// The path to the Aspire manifest
    /// </summary>
    [CommandOption("-p|--project")]
    [Description("The path to the Aspire AppHost project")]
    public string PathToAspireProjectFlag { get; init; } = AspirateLiterals.DefaultAspireProjectPath;

    /// <summary>
    /// The path to the output kustomize manifest
    /// </summary>
    [CommandOption("-o|--output")]
    [Description("The output path for the generated kustomize manifests")]
    public string OutputPathFlag { get; init; } = AspirateLiterals.DefaultOutputPath;
}
