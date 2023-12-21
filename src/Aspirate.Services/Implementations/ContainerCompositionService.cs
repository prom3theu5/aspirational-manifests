namespace Aspirate.Services.Implementations;

public sealed class ContainerCompositionService(
    IFileSystem filesystem,
    IAnsiConsole console,
    IProjectPropertyService projectPropertyService,
    IShellExecutionService shellExecutionService) : IContainerCompositionService
{
    public async Task<bool> BuildAndPushContainerForProject(
        ProjectResource projectResource,
        MsBuildContainerProperties containerDetails,
        string builder,
        bool nonInteractive = false)
    {
        await CheckIfBuilderIsRunning(builder);

        var fullProjectPath = filesystem.NormalizePath(projectResource.Path);

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

    public async Task<bool> BuildAndPushContainerForDockerfile(DockerfileResource dockerfileResource, string builder, string imageName, string? registry, bool nonInteractive)
    {
        await CheckIfBuilderIsRunning(builder);

        var tagBuilder = new StringBuilder();
        var fullDockerfilePath = filesystem.GetFullPath(dockerfileResource.Path);

        if (!string.IsNullOrEmpty(registry))
        {
            tagBuilder.Append($"{registry}/");
        }

        tagBuilder.Append(imageName);
        tagBuilder.Append(":latest");

        var tag = tagBuilder.ToString();

        var result = await BuildContainer(dockerfileResource, builder, nonInteractive, tag, fullDockerfilePath);

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

    private Task<ShellCommandResult> BuildContainer(DockerfileResource dockerfileResource, string builder, bool nonInteractive, string tag, string fullDockerfilePath)
    {
        var buildArgumentBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerLiterals.BuildCommand, string.Empty, quoteValue: false)
            .AppendArgument(DockerLiterals.TagArgument, tag.ToLower());


        if (dockerfileResource.Env is not null)
        {
            AddDockerBuildArgs(buildArgumentBuilder, dockerfileResource.Env);
        }

        buildArgumentBuilder
            .AppendArgument(DockerLiterals.DockerFileArgument, fullDockerfilePath)
            .AppendArgument(dockerfileResource.Context, string.Empty, quoteValue: false);

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
        var builderAvailable = await shellExecutionService.IsCommandAvailable(builder);

        if (builderAvailable is null || !builderAvailable.IsAvailable)
        {
            console.MarkupLine($"\r\n[red bold]{builder} is not available or found on your system.[/]");
            throw new ActionCausesExitException(1);
        }

        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("info", string.Empty, quoteValue: false)
            .AppendArgument("--format", "json", quoteValue: false);

        var builderCheckResult = await shellExecutionService.ExecuteCommand(new()
        {
            Command = builderAvailable.FullPath,
            ArgumentsBuilder = argumentsBuilder,
        });

        ValidateBuilderOutput(builderCheckResult);
    }

    private void ValidateBuilderOutput(ShellCommandResult builderCheckResult)
    {
        var builderInfo = JsonDocument.Parse(builderCheckResult.Output);

        if (!builderInfo.RootElement.TryGetProperty("ServerErrors", out var errorProperty))
        {
            return;
        }

        if (errorProperty.ValueKind == JsonValueKind.Array && errorProperty.GetArrayLength() == 0)
        {
            return;
        }

        string messages = string.Join(Environment.NewLine, errorProperty.EnumerateArray());
        console.MarkupLine("[red][bold]The daemon server reported errors:[/][/]");
        console.MarkupLine($"[red]{messages}[/]");
        throw new ActionCausesExitException(1);
    }

    private static void CheckSuccess(ShellCommandResult result)
    {
        if (result.ExitCode != 0)
        {
            throw new ActionCausesExitException(9999);
        }
    }
}
