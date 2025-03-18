namespace Aspirate.Commands.Commands.Stop;

public sealed class StopOptions : BaseCommandOptions,
    IMinikubeOptions
{
    public bool? DisableMinikubeMountAction { get; set; }
}
