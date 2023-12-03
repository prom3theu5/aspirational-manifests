namespace Aspirate.CommandSupport.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptions : ICommandOptions
{
    public bool NonInteractive { get; set; } = false;
}
