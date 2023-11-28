namespace Aspirate.Shared.Commands;

public abstract class BaseCommandOptions : ICommandOptions
{
    public bool NonInteractive { get; set; } = false;
}
