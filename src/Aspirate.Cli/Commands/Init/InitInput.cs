namespace Aspirate.Cli.Commands.Init;

/// <summary>
/// The input for the EndToEndCommand
/// </summary>
public sealed class InitInput : CommandSettings
{
    /// <summary>
    /// The path to the Aspire manifest
    /// </summary>
    [CommandOption("-p|--project")]
    [Description("The path to the Aspire AppHost project")]
    public string PathToAspireProjectFlag { get; init; } = ".";
}
