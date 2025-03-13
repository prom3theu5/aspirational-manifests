namespace Aspirate.Commands.Commands.Stop;

public sealed class StopOptions : BaseCommandOptions,
    IMinikubeOptions
{
    public bool? EnableMinikubeMountAction { get; set; }
}
