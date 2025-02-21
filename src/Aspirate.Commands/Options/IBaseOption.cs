namespace Aspirate.Commands.Options;

public interface IBaseOption
{
    bool IsSecret { get; }

    public object? GetOptionDefault();
}
