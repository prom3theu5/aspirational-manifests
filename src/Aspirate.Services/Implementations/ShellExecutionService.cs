namespace Aspirate.Services.Implementations;

public class ShellExecutionService(IAnsiConsole console, IFileSystem fileSystem) : IShellExecutionService
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();
    private bool _showOutput;

    public async Task<ShellCommandResult> ExecuteCommand(ShellCommandOptions options)
    {
        _showOutput = options.ShowOutput;
        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var arguments = options.ArgumentsBuilder.RenderArguments(propertyKeySeparator: options.PropertyKeySeparator);

        if (options.ShowOutput)
        {
            console.WriteLine();
            console.MarkupLine(string.IsNullOrEmpty(options.PreCommandMessage) ? $"[cyan]Executing: {options.Command} {arguments}[/]" : options.PreCommandMessage);
        }

        var executionDirectory = string.IsNullOrEmpty(options.WorkingDirectory)
            ? fileSystem.Directory.GetCurrentDirectory()
            : options.WorkingDirectory;

        var result = await CliWrap.Cli.Wrap(options.Command)
            .WithWorkingDirectory(executionDirectory)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(WriteInfo))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(WriteError))
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0)
        {
            await HandleExitCode(options.Command, options.ArgumentsBuilder, options.NonInteractive, options.OnFailed, options.FailureCommandMessage, result, options.ExitWithExitCode);
        }

        if (result.ExitCode == 0)
        {
            if (!string.IsNullOrEmpty(options.SuccessCommandMessage))
            {
                console.MarkupLine(options.SuccessCommandMessage);
            }
        }

        return new(result.ExitCode == 0, result.StandardOutput, result.StandardError, result.ExitCode);
    }

    public async Task<bool> ExecuteCommandWithEnvironmentNoOutput(string command, ArgumentsBuilder argumentsBuilder, IReadOnlyDictionary<string, string?> environmentVariables)
    {
        try
        {
            var arguments = argumentsBuilder.RenderArguments();

            var executionCommand = Cli.Wrap(command)
                .WithArguments(arguments)
                .WithEnvironmentVariables(environmentVariables);

            var commandResult = await executionCommand
                .WithValidation(CommandResultValidation.ZeroExitCode)
                .WithStandardErrorPipe(PipeTarget.Null)
                .WithStandardOutputPipe(PipeTarget.Null)
                .ExecuteAsync();

            return commandResult.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
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
