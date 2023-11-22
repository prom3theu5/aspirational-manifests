namespace Aspirate.Cli.Services;

public sealed class ContainerCompositionService(IFileSystem filesystem) : IContainerCompositionService
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();

    public async Task<bool> BuildAndPushContainerForProject(Project project)
    {
        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var currentDirectory = filesystem.Directory.GetCurrentDirectory();
        var normalizedProjectPath = project.Path.Replace('\\', filesystem.Path.DirectorySeparatorChar);
        var fullProjectPath = filesystem.Path.Combine(currentDirectory, normalizedProjectPath);

        await ExecuteCommand(
            ContainerBuilderLiterals.DotNetCommand,
            fullProjectPath,
            onFailed: HandleBuildErrors);

        return true;
    }

    private async Task ExecuteCommand(string command, string projectPath, string? arguments = null, Func<string, string, string, Task>? onFailed = default)
    {
        if (string.IsNullOrEmpty(arguments))
        {
            arguments = ContainerBuilderLiterals.DefaultBuildArguments(projectPath);
        }

        var executeCommand = CliWrap.Cli.Wrap(command)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(_stdErrBuffer));

        await foreach(var cmdEvent in executeCommand.ListenAsync())
        {
            switch (cmdEvent)
            {
                case StartedCommandEvent _:
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine($"[cyan](⚙) Executing: {command} {arguments}[/]");
                    break;
                case StandardOutputCommandEvent stdOut:
                    AnsiConsole.WriteLine(stdOut.Text);
                    break;
                case StandardErrorCommandEvent stdErr:
                    AnsiConsole.MarkupLine($"[red]{stdErr.Text}[/]");
                    break;
                case ExitedCommandEvent exited:
                    if (exited.ExitCode != 0)
                    {
                        await onFailed?.Invoke(command, projectPath, _stdErrBuffer.Append(_stdOutBuffer).ToString());
                    }
                    break;
            }
        }

        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();
    }

    private static async Task<bool> ExecuteCommandNoOutput(string command, IReadOnlyDictionary<string, string?> environmentVariables)
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

    private Task HandleBuildErrors(string command, string fullProjectPath, string errors)
    {
        if (errors.Contains(DotNetSdkLiterals.DuplicateFileOutputError, StringComparison.OrdinalIgnoreCase))
        {
            return HandleDuplicateFilesInOutput(command, fullProjectPath);
        }

        if (errors.Contains(DotNetSdkLiterals.NoContainerRegistryAccess, StringComparison.OrdinalIgnoreCase))
        {
            return HandleNoDockerRegistryAccess(command, fullProjectPath);
        }

        if (errors.Contains(DotNetSdkLiterals.UnknownContainerRegistryAddress, StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine($"\r\n[red bold]{DotNetSdkLiterals.UnknownContainerRegistryAddress}: Unknown container registry address, or container registry address not accessible.[/]");

            Environment.Exit(1013);
        }

        return Task.CompletedTask;
    }

    private Task HandleDuplicateFilesInOutput(string command, string projectPath)
    {
        var shouldRetry = AskIfShouldRetryHandlingDuplicateFiles();
        if (shouldRetry)
        {
            var arguments = ContainerBuilderLiterals.DuplicateFileOutputBuildArguments(projectPath);

            _stdErrBuffer.Clear();
            _stdOutBuffer.Clear();

            return ExecuteCommand(command, projectPath, arguments, HandleBuildErrors);
        }

        return Task.CompletedTask;
    }

    private async Task HandleNoDockerRegistryAccess(string command, string projectPath)
    {
        var shouldLogin = AskIfShouldLoginToDocker();
        if (shouldLogin)
        {
            var credentials = GatherDockerCredentials();

            var loginResult = await ExecuteCommandNoOutput(ContainerBuilderLiterals.DockerLoginCommand, credentials);

            if (loginResult)
            {
                await ExecuteCommand(
                    command,
                    projectPath,
                    onFailed: HandleBuildErrors);
            }
        }
    }

    private static bool AskIfShouldRetryHandlingDuplicateFiles() =>
        AnsiConsole.Confirm("\r\n[red bold]Implicitly, dotnet publish does not allow duplicate filenames to be output to the artefact directory at build time.\r\nWould you like to retry the build explicitly allowing them?[/]\r\n");

    private static bool AskIfShouldLoginToDocker() =>
        AnsiConsole.Confirm("\r\nWe could not access the container registry during build. Do you want to login to the registry and retry?\r\n");

    private static Dictionary<string, string?> GatherDockerCredentials()
    {
        AnsiConsole.WriteLine();
        var registry = AnsiConsole.Ask<string>("What's the registry [green]address[/]?");
        var username = AnsiConsole.Ask<string>("Enter [green]username[/]?");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]password[/]?")
                .PromptStyle("red")
                .Secret());

        return new()
        {
            { "DOCKER_HOST", registry },
            { "DOCKER_USER", username },
            { "DOCKER_PASS", password },
        };
    }
}
