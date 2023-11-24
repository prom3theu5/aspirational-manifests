namespace Aspirate.Cli.Actions;

public interface IAction
{
    Task<bool> ExecuteAsync();
}
