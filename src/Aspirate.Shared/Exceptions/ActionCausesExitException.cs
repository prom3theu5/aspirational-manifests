namespace Aspirate.Shared.Exceptions;

public class ActionCausesExitException(int exitCode, string? message = null) : Exception(message)
{
    public int ExitCode { get; } = exitCode;

    [DoesNotReturn]
    public static void ExitNow(int exitCode = 1, string? message = null) => throw new ActionCausesExitException(exitCode);
}
