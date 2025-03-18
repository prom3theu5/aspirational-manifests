namespace Aspirate.Commands.Commands.Stop;

public sealed class StopCommand : BaseCommand<StopOptions, StopCommandHandler>
{
    protected override bool CommandUnlocksSecrets => false;
    protected override bool CommandAlwaysRequiresState => true;

    public StopCommand() : base("stop", "Stops a deployment that has been made using 'aspirate run' by destroying it.")
    {
        AddOption(DisableMinikubeMountActionOption.Instance);
    }
}
