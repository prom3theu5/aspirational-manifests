namespace Aspirate.Commands.Actions.Secrets;

public class SaveInputsAction(
    IAnsiConsole console,
    IPasswordGenerator passwordGenerator,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        return Task.FromResult(true);
    }
}
