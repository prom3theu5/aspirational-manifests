namespace Aspirate.Cli.Exceptions;

public class ActionCausesExitException(int exitCode) : Exception
{
    public int ExitCode { get; } = exitCode;
}
