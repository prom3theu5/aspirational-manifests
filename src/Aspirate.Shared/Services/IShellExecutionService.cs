namespace Aspirate.Shared.Services;

public interface IShellExecutionService
{
    Task<ShellCommandResult> ExecuteCommand(string command, ArgumentsBuilder argumentsBuilder, bool nonInteractive = false,
        Func<string, ArgumentsBuilder, bool, string, Task>? onFailed = default,
        bool? showOutput = true,
        string? workingDirectory = null,
        char? propertyKeySeparator = null,
        string? preCommandMessage = null,
        string? successCommandMessage = null,
        string? failureCommandMessage = null,
        bool exitWithExitCode = false);

    Task<bool> ExecuteCommandWithEnvironmentNoOutput(string command, IReadOnlyDictionary<string, string?> environmentVariables);
}
