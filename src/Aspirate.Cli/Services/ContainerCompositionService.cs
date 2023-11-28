using CliWrap.Buffered;

namespace Aspirate.Cli.Services;

public sealed class ContainerCompositionService(IFileSystem filesystem, IAnsiConsole console, IProjectPropertyService projectPropertyService) : IContainerCompositionService
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();

    public async Task<bool> BuildAndPushContainerForProject(
        Project project,
        MsBuildContainerProperties containerDetails,
        bool nonInteractive)
    {
        var fullProjectPath = filesystem.NormalizePath(project.Path);

        var argumentsBuilder = ArgumentsBuilder.Create();

        await AddProjectPublishArguments(argumentsBuilder, fullProjectPath);
        AddContainerDetailsToArguments(argumentsBuilder, containerDetails);

        await ExecuteCommand(DotNetSdkLiterals.DotNetCommand, argumentsBuilder, nonInteractive, onFailed: HandleBuildErrors);

        return true;
    }

    public async Task<bool> BuildAndPushContainerForDockerfile(Dockerfile dockerfile, string builder, string imageName, string registry, bool nonInteractive)
    {
        var fullDockerfilePath = filesystem.GetFullPath(dockerfile.Path);

        var tag = $"{registry}/{imageName}:latest";

        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerLiterals.BuildCommand, string.Empty, quoteValue: false)
            .AppendArgument(DockerLiterals.TagArgument, tag)
            .AppendArgument(DockerLiterals.DockerFileArgument, fullDockerfilePath)
            .AppendArgument(dockerfile.Context, string.Empty, quoteValue: false);

        await ExecuteCommand(builder, argumentsBuilder, nonInteractive);

        argumentsBuilder.Clear()
            .AppendArgument(DockerLiterals.PushCommand, string.Empty, quoteValue: false)
            .AppendArgument(tag, string.Empty, quoteValue: false);

        await ExecuteCommand(builder, argumentsBuilder, nonInteractive);

        return true;
    }

    private async Task ExecuteCommand(
        string command,
        ArgumentsBuilder argumentsBuilder,
        bool nonInteractive,
        Func<string, ArgumentsBuilder, bool, string, Task>? onFailed = default)
    {
        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var arguments = argumentsBuilder.RenderArguments();

        console.WriteLine();
        console.MarkupLine($"[cyan]Executing: {command} {arguments}[/]");

        var result = await CliWrap.Cli.Wrap(command)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(WriteInfo))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(WriteError))
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0)
        {
            await onFailed?.Invoke(
                           command,
                           argumentsBuilder,
                           nonInteractive,
                           _stdErrBuffer.Append(_stdOutBuffer).ToString());
        }
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

    private Task HandleBuildErrors(string command, ArgumentsBuilder argumentsBuilder, bool nonInteractive, string errors)
    {
        if (errors.Contains(DotNetSdkLiterals.DuplicateFileOutputError, StringComparison.OrdinalIgnoreCase))
        {
            return HandleDuplicateFilesInOutput(argumentsBuilder, nonInteractive);
        }

        if (errors.Contains(DotNetSdkLiterals.NoContainerRegistryAccess, StringComparison.OrdinalIgnoreCase))
        {
            return HandleNoDockerRegistryAccess(argumentsBuilder, nonInteractive);
        }

        if (errors.Contains(DotNetSdkLiterals.UnknownContainerRegistryAddress, StringComparison.OrdinalIgnoreCase))
        {
            console.MarkupLine($"\r\n[red bold]{DotNetSdkLiterals.UnknownContainerRegistryAddress}: Unknown container registry address, or container registry address not accessible.[/]");
            throw new ActionCausesExitException(1013);
        }

        throw new ActionCausesExitException(9999);
    }

    private Task HandleDuplicateFilesInOutput(ArgumentsBuilder argumentsBuilder, bool nonInteractive)
    {
        var shouldRetry = AskIfShouldRetryHandlingDuplicateFiles(nonInteractive);
        if (shouldRetry)
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.ErrorOnDuplicatePublishOutputFilesArgument, "false");

           return ExecuteCommand(DotNetSdkLiterals.DotNetCommand, argumentsBuilder, nonInteractive, HandleBuildErrors);
        }

        throw new ActionCausesExitException(9999);
    }

    private async Task HandleNoDockerRegistryAccess(ArgumentsBuilder argumentsBuilder, bool nonInteractive)
    {
        if (nonInteractive)
        {
            console.MarkupLine($"\r\n[red bold]{DotNetSdkLiterals.NoContainerRegistryAccess}: No access to container registry. Cannot attempt login in non interactive mode.[/]");
            throw new ActionCausesExitException(1000);
        }

        var shouldLogin = AskIfShouldLoginToDocker(nonInteractive);
        if (shouldLogin)
        {
            var credentials = GatherDockerCredentials();

            var loginResult = await ExecuteCommandNoOutput(ContainerBuilderLiterals.DockerLoginCommand, credentials);

            if (loginResult)
            {
                await ExecuteCommand(
                    DotNetSdkLiterals.DotNetCommand,
                    argumentsBuilder,
                    nonInteractive,
                    onFailed: HandleBuildErrors);
            }
        }
    }

    private bool AskIfShouldRetryHandlingDuplicateFiles(bool nonInteractive)
    {
        if (nonInteractive)
        {
            return true;
        }

        return console.Confirm(
            "\r\n[red bold]Implicitly, dotnet publish does not allow duplicate filenames to be output to the artefact directory at build time.\r\nWould you like to retry the build explicitly allowing them?[/]\r\n");
    }

    private bool AskIfShouldLoginToDocker(bool nonInteractive)
    {
        if (nonInteractive)
        {
            return false;
        }

        return console.Confirm(
            "\r\nWe could not access the container registry during build. Do you want to login to the registry and retry?\r\n");
    }

    private Dictionary<string, string?> GatherDockerCredentials()
    {
        console.WriteLine();
        var registry = console.Ask<string>("What's the registry [green]address[/]?");
        var username = console.Ask<string>("Enter [green]username[/]?");
        var password = console.Prompt(
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

    private async Task AddProjectPublishArguments(ArgumentsBuilder argumentsBuilder, string fullProjectPath)
    {
        var propertiesJson = await projectPropertyService.GetProjectPropertiesAsync(
            fullProjectPath,
            MsBuildPropertiesLiterals.PublishSingleFileArgument,
            MsBuildPropertiesLiterals.PublishTrimmedArgument);

        var msbuildProperties = JsonSerializer.Deserialize<MsBuildProperties<MsBuildPublishingProperties>>(propertiesJson ?? "{}");

        if (string.IsNullOrEmpty(msbuildProperties.Properties.PublishSingleFile))
        {
            msbuildProperties.Properties.PublishSingleFile = DotNetSdkLiterals.DefaultSingleFile;
        }

        if (string.IsNullOrEmpty(msbuildProperties.Properties.PublishTrimmed))
        {
            msbuildProperties.Properties.PublishTrimmed = DotNetSdkLiterals.DefaultPublishTrimmed;
        }

        argumentsBuilder
            .AppendArgument(DotNetSdkLiterals.PublishArgument, fullProjectPath)
            .AppendArgument(DotNetSdkLiterals.PublishProfileArgument, DotNetSdkLiterals.ContainerPublishProfile)
            .AppendArgument(DotNetSdkLiterals.PublishSingleFileArgument, msbuildProperties.Properties.PublishSingleFile)
            .AppendArgument(DotNetSdkLiterals.PublishTrimmedArgument, msbuildProperties.Properties.PublishTrimmed)
            .AppendArgument(DotNetSdkLiterals.SelfContainedArgument, DotNetSdkLiterals.DefaultSelfContained)
            .AppendArgument(DotNetSdkLiterals.OsArgument, DotNetSdkLiterals.DefaultOs)
            .AppendArgument(DotNetSdkLiterals.ArchArgument, DotNetSdkLiterals.DefaultArch);

    }

    private static void AddContainerDetailsToArguments(ArgumentsBuilder argumentsBuilder, MsBuildContainerProperties containerDetails)
    {
        argumentsBuilder.AppendArgument(DotNetSdkLiterals.ContainerRegistryArgument, containerDetails.ContainerRegistry);

        if (!string.IsNullOrEmpty(containerDetails.ContainerRepository))
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.ContainerRepositoryArgument, containerDetails.ContainerRepository);
        }

        if (!string.IsNullOrEmpty(containerDetails.ContainerImageName))
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.ContainerImageNameArgument, containerDetails.ContainerImageName);
        }

        argumentsBuilder.AppendArgument(DotNetSdkLiterals.ContainerImageTagArgument, containerDetails.ContainerImageTag);
    }

    private void WriteError(string message)
    {
        console.MarkupLine("[red]{0}[/]", message.EscapeMarkup());
        _stdErrBuffer.AppendLine(message);
    }

    private void WriteInfo(string message)
    {
        console.WriteLine(message);
        _stdOutBuffer.AppendLine(message);
    }
}
