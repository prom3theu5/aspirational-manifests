namespace Aspirate.Services.Implementations;

public sealed class ContainerCompositionService(
    IFileSystem filesystem,
    IAnsiConsole console,
    IProjectPropertyService projectPropertyService,
    IShellExecutionService shellExecutionService) : IContainerCompositionService
{
    public async Task<bool> BuildAndPushContainerForProject(
        Project project,
        MsBuildContainerProperties containerDetails,
        bool nonInteractive = false)
    {
        var fullProjectPath = filesystem.NormalizePath(project.Path);

        var argumentsBuilder = ArgumentsBuilder.Create();

        await AddProjectPublishArguments(argumentsBuilder, fullProjectPath);
        AddContainerDetailsToArguments(argumentsBuilder, containerDetails);

        await shellExecutionService.ExecuteCommand(new()
        {
            Command = DotNetSdkLiterals.DotNetCommand,
            ArgumentsBuilder = argumentsBuilder,
            NonInteractive = nonInteractive,
            OnFailed = HandleBuildErrors,
            ShowOutput = true,
        });

        return true;
    }

    public async Task<bool> BuildAndPushContainerForDockerfile(Dockerfile dockerfile, string builder, string imageName, string? registry, bool nonInteractive)
    {
        await CheckIfBuilderIsRunning(builder);

        var tagBuilder = new StringBuilder();
        var fullDockerfilePath = filesystem.GetFullPath(dockerfile.Path);

        if (!string.IsNullOrEmpty(registry))
        {
            tagBuilder.Append($"{registry}/");
        }

        tagBuilder.Append(imageName);
        tagBuilder.Append(":latest");

        var tag = tagBuilder.ToString();

        var result = await BuildContainer(dockerfile, builder, nonInteractive, tag, fullDockerfilePath);

        CheckSuccess(result);

        result = await PushContainer(builder, registry, nonInteractive, tag);

        CheckSuccess(result);

        return true;
    }

    private async Task<ShellCommandResult> PushContainer(string builder, string? registry, bool nonInteractive, string tag)
    {
        if (!string.IsNullOrEmpty(registry))
        {
            var pushArgumentBuilder = ArgumentsBuilder
                .Create()
                .AppendArgument(DockerLiterals.PushCommand, string.Empty, quoteValue: false)
                .AppendArgument(tag.ToLower(), string.Empty, quoteValue: false);

            return await shellExecutionService.ExecuteCommand(
                new()
                {
                    Command = builder,
                    ArgumentsBuilder = pushArgumentBuilder,
                    NonInteractive = nonInteractive,
                    ShowOutput = true,
                });
        }

        return new ShellCommandResult(true, string.Empty, string.Empty, 0);
    }

    private Task<ShellCommandResult> BuildContainer(Dockerfile dockerfile, string builder, bool nonInteractive, string tag, string fullDockerfilePath)
    {
        var buildArgumentBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerLiterals.BuildCommand, string.Empty, quoteValue: false)
            .AppendArgument(DockerLiterals.TagArgument, tag.ToLower());


        if (dockerfile.Env is not null)
        {
            AddDockerBuildArgs(buildArgumentBuilder, dockerfile.Env);
        }

        buildArgumentBuilder
            .AppendArgument(DockerLiterals.DockerFileArgument, fullDockerfilePath)
            .AppendArgument(dockerfile.Context, string.Empty, quoteValue: false);

        return shellExecutionService.ExecuteCommand(new()
        {
            Command = builder,
            ArgumentsBuilder = buildArgumentBuilder,
            NonInteractive = nonInteractive,
            ShowOutput = true,
        });
    }

    private Task HandleBuildErrors(string command, ArgumentsBuilder argumentsBuilder, bool nonInteractive, string errors)
    {
        if (errors.Contains(DotNetSdkLiterals.DuplicateFileOutputError, StringComparison.OrdinalIgnoreCase))
        {
            return HandleDuplicateFilesInOutput(argumentsBuilder, nonInteractive);
        }

        if (errors.Contains(DotNetSdkLiterals.UnknownContainerRegistryAddress, StringComparison.OrdinalIgnoreCase))
        {
            console.MarkupLine($"\r\n[red bold]{DotNetSdkLiterals.UnknownContainerRegistryAddress}: Unknown container registry address, or container registry address not accessible.[/]");
            throw new ActionCausesExitException(1013);
        }

        throw new ActionCausesExitException(9999);
    }

    private Task HandleDuplicateFilesInOutput(ArgumentsBuilder argumentsBuilder, bool nonInteractive = false)
    {
        var shouldRetry = AskIfShouldRetryHandlingDuplicateFiles(nonInteractive);
        if (shouldRetry)
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.ErrorOnDuplicatePublishOutputFilesArgument, "false");

            return shellExecutionService.ExecuteCommand(new()
            {
                Command = DotNetSdkLiterals.DotNetCommand,
                ArgumentsBuilder = argumentsBuilder,
                NonInteractive = nonInteractive,
                OnFailed = HandleBuildErrors,
                ShowOutput = true,
            });
        }

        throw new ActionCausesExitException(9999);
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
        if (!string.IsNullOrEmpty(containerDetails.ContainerRegistry))
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.ContainerRegistryArgument, containerDetails.ContainerRegistry);
        }

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

    private static void AddDockerBuildArgs(ArgumentsBuilder argumentsBuilder, Dictionary<string, string> dockerfileEnv)
    {
        foreach (var (key, value) in dockerfileEnv)
        {
            argumentsBuilder.AppendArgument(DockerLiterals.BuildArgArgument, $"{key}=\"{value}\"", quoteValue: false, allowDuplicates: true);
        }
    }

    private async Task CheckIfBuilderIsRunning(string builder)
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("info", string.Empty, quoteValue: false);

        var builderIsRunning = await shellExecutionService.ExecuteCommandWithEnvironmentNoOutput(builder, argumentsBuilder, new Dictionary<string, string?>());
        if (!builderIsRunning)
        {
            console.MarkupLine($"\r\n[red bold]{builder} is not running.[/]");
            throw new ActionCausesExitException(1);
        }
    }

    private static void CheckSuccess(ShellCommandResult result)
    {
        if (result.ExitCode != 0)
        {
            throw new ActionCausesExitException(9999);
        }
    }
}
