namespace Aspirate.Shared.Exceptions;

public class ActionCausesExitException(int exitCode) : Exception
{
    public int ExitCode { get; } = exitCode;

    public static void ExitNow(int exitCode = 1) => throw new ActionCausesExitException(exitCode);
}
