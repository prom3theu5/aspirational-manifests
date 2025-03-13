namespace Aspirate.Shared.Outputs;

public record ShellCommandResult(bool Success, string Output, string Error, int ExitCode);
