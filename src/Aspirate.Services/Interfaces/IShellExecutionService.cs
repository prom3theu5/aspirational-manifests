namespace Aspirate.Services.Interfaces;

/// <summary>
/// Represents a service for executing shell commands.
/// </summary>
public interface IShellExecutionService
{
    /// <summary>
    /// Executes a shell command with the given options.
    /// </summary>
    /// <param name="options">The options for the shell command.</param>
    /// <returns>
    /// A <see cref="Task{ShellCommandResult}"/> representing the asynchronous operation.
    /// The task will contain the result of the command execution, encapsulated in a <see cref="ShellCommandResult"/> object.
    /// </returns>
    Task<ShellCommandResult> ExecuteCommand(ShellCommandOptions options);

    /// <summary>
    /// Executes a command with environment variables and no output.
    /// </summary>
    /// <param name="command">
    /// The command to execute.
    /// </param>
    /// <param name="argumentsBuilder">The arguments builder instance.</param>
    /// <param name="environmentVariables">
    /// The dictionary of environment variables to set for the command.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution of the command.
    /// The task result is a boolean indicating whether the command was executed successfully or not.
    /// </returns>
    Task<bool> ExecuteCommandWithEnvironmentNoOutput(string command, ArgumentsBuilder argumentsBuilder,
        IReadOnlyDictionary<string, string?> environmentVariables);

    /// <summary>
    /// Determines if a specific command is available.
    /// </summary>
    /// <param name="commandName">The name of the command to check availability for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains
    /// true if the command is available, false otherwise.</returns>
    CommandAvailableResult IsCommandAvailable(string commandName);
}
