namespace Aspirate.CommandSupport.Commands;

public interface ICommandOptionsHandler<in TOptions>
{
    AspirateState CurrentState { get; set; }

    Task<int> HandleAsync(TOptions options);
}
