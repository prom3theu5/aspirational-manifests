namespace Aspirate.Services.Interfaces;

public interface IShellExecutionService
{
    Task<ShellCommandResult> ExecuteCommand(ShellCommandOptions options);

    Task<bool> ExecuteCommandWithEnvironmentNoOutput(string command, IReadOnlyDictionary<string, string?> environmentVariables);
}
