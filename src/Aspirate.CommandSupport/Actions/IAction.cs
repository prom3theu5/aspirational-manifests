namespace Aspirate.CommandSupport.Actions;

public interface IAction
{
    Task<bool> ExecuteAsync();
}
