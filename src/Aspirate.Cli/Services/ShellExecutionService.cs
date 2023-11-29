namespace Aspirate.Cli.Services;

public class ShellExecutionService(IAnsiConsole console, IFileSystem fileSystem) : IShellExecutionService
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();
    private bool _showOutput;

    public async Task<ShellCommandResult> ExecuteCommand(string command,
        ArgumentsBuilder argumentsBuilder,
        bool nonInteractive = false,
        Func<string, ArgumentsBuilder, bool, string, Task>? onFailed = default,
        bool? showOutput = true,
        string? workingDirectory = null,
        char? propertyKeySeparator = null,
        string? preCommandMessage = null,
        string? successCommandMessage = null,
        string? failureCommandMessage = null,
        bool exitWithExitCode = false)
    {
        _showOutput = showOutput ?? true;
        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var arguments = argumentsBuilder.RenderArguments(propertyKeySeparator: propertyKeySeparator ?? ' ');

        if (showOutput == true)
        {
            console.WriteLine();
            console.MarkupLine(string.IsNullOrEmpty(preCommandMessage) ? $"[cyan]Executing: {command} {arguments}[/]" : preCommandMessage);
        }

        var executionDirectory = string.IsNullOrEmpty(workingDirectory)
            ? fileSystem.Directory.GetCurrentDirectory()
            : workingDirectory;

        var result = await CliWrap.Cli.Wrap(command)
            .WithWorkingDirectory(executionDirectory)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(WriteInfo))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(WriteError))
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0)
        {
            await HandleExitCode(command, argumentsBuilder, nonInteractive, onFailed, failureCommandMessage, result, exitWithExitCode);
        }

        if (result.ExitCode == 0)
        {
            if (!string.IsNullOrEmpty(successCommandMessage))
            {
                console.MarkupLine(successCommandMessage);
            }
        }

        return new(result.ExitCode == 0, result.StandardOutput, result.StandardError, result.ExitCode);
    }

    public async Task<bool> ExecuteCommandWithEnvironmentNoOutput(string command, IReadOnlyDictionary<string, string?> environmentVariables)
    {
        var executionCommand = CliWrap.Cli.Wrap(command)
            .WithEnvironmentVariables(environmentVariables);

        var commandResult = await executionCommand
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .WithStandardErrorPipe(PipeTarget.Null)
            .WithStandardOutputPipe(PipeTarget.Null)
            .ExecuteAsync();

        return commandResult.ExitCode != 0;
    }

    private Task HandleExitCode(string command,
        ArgumentsBuilder argumentsBuilder,
        bool nonInteractive,
        Func<string, ArgumentsBuilder, bool, string, Task>? onFailed,
        string? failureCommandMessage, BufferedCommandResult result,
        bool exitWithExitCode)
    {
        if (onFailed != null)
        {
            return onFailed.Invoke(
                command,
                argumentsBuilder,
                nonInteractive,
                _stdErrBuffer.Append(_stdOutBuffer).ToString());
        }

        if (!string.IsNullOrEmpty(failureCommandMessage))
        {
            console.MarkupLine(failureCommandMessage);
        }

        if (exitWithExitCode)
        {
            throw new ActionCausesExitException(result.ExitCode);
        }

        return Task.CompletedTask;
    }

    private void WriteError(string message)
    {
        if (_showOutput)
        {
            console.MarkupLine("[red]{0}[/]", message.EscapeMarkup());
        }

        _stdErrBuffer.AppendLine(message);
    }

    private void WriteInfo(string message)
    {
        if (_showOutput)
        {
            console.WriteLine(message);
        }

        _stdOutBuffer.AppendLine(message);
    }
}
