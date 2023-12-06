namespace Aspirate.Commands.Actions;

public interface IAction
{
    Task<bool> ExecuteAsync();
}
