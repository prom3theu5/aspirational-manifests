namespace Aspirate.Cli.Commands.Init;

public sealed class InitInput : CommandSettings
{
    /// <summary>
    /// The path to the Aspire manifest
    /// </summary>
    [CommandOption("-p|--project")]
    [Description("The path to the Aspire AppHost project")]
    public string PathToAspireProjectFlag { get; init; } = AspirateLiterals.DefaultAspireProjectPath;
}
