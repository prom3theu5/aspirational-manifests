namespace Aspirate.Commands.Commands.Stop;

public sealed class StopCommand() : BaseCommand<StopOptions, StopCommandHandler>("stop", "Stops a deployment that has been made using 'aspirate run' by destroying it.")
{
    protected override bool CommandUnlocksSecrets => false;
    protected override bool CommandAlwaysRequiresState => true;
}
