namespace Aspirate.Shared.Actions;

public interface IAction
{
    Task<bool> ExecuteAsync();
}
