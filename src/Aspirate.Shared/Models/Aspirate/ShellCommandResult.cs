namespace Aspirate.Shared.Models.Aspirate;

public record ShellCommandResult(bool Success, string Output, string Error, int ExitCode);
