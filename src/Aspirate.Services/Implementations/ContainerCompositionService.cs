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
        ContainerOptions options,
        bool nonInteractive = false,
        string? runtimeIdentifier = null)
    {
        await CheckIfBuilderIsRunning(options.ContainerBuilder);

        var fullProjectPath = filesystem.NormalizePath(projectResource.Path);

        var argumentsBuilder = ArgumentsBuilder.Create();

        if (!string.IsNullOrEmpty(options.Prefix))
        {
            containerDetails.ContainerRepository = $"{options.Prefix}/{containerDetails.ContainerRepository}";
        }

        AddProjectPublishArguments(argumentsBuilder, fullProjectPath, runtimeIdentifier);
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

    public async Task<bool> BuildAndPushContainerForDockerfile(DockerfileResource dockerfileResource, ContainerOptions options, bool? nonInteractive = false) =>
        await BuildAndPushContainerForDockerfile(
            dockerfileResource.Context,
            dockerfileResource.Env,
            dockerfileResource.BuildArgs,
            dockerfileResource.Path,
            dockerfileResource.Name,
            options,
            nonInteractive);

    public async Task<bool> BuildAndPushContainerForDockerfile(ContainerV1Resource containerV1Resource, ContainerOptions options, bool? nonInteractive = false) =>
        await BuildAndPushContainerForDockerfile(
            containerV1Resource.Build.Context,
            containerV1Resource.Env,
            containerV1Resource.Build.Args,
            containerV1Resource.Build.Dockerfile,
            containerV1Resource.Name,
            options,
            nonInteractive);

    private async Task<bool> BuildAndPushContainerForDockerfile(string context, Dictionary<string, string>? env, Dictionary<string, string> buildArgs, string dockerfile, string resourceName, ContainerOptions options, bool? nonInteractive = false)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        await CheckIfBuilderIsRunning(options.ContainerBuilder);

        var fullDockerfilePath = filesystem.GetFullPath(dockerfile);

        var fullImages = options.ToImageNames(resourceName);

        var result = await BuildContainer(context, env, buildArgs, options.ContainerBuilder, nonInteractive, fullImages, fullDockerfilePath);

        CheckSuccess(result);

        result = await PushContainer(options.ContainerBuilder, options.Registry, fullImages, nonInteractive);

        CheckSuccess(result);

        return true;
    }

    private async Task<ShellCommandResult> PushContainer(string builder, string? registry, List<string> fullImages, bool? nonInteractive)
    {
        if (!string.IsNullOrEmpty(registry))
        {
            ShellCommandResult? result = null;

            foreach (var fullImage in fullImages)
            {
                var pushArgumentBuilder = ArgumentsBuilder
                    .Create()
                    .AppendArgument(DockerLiterals.PushCommand, string.Empty, quoteValue: false);

                pushArgumentBuilder.AppendArgument(fullImage.ToLower(), string.Empty, quoteValue: false, allowDuplicates: true);

                result = await shellExecutionService.ExecuteCommand(
                    new()
                    {
                        Command = builder,
                        ArgumentsBuilder = pushArgumentBuilder,
                        NonInteractive = nonInteractive.GetValueOrDefault(),
                        ShowOutput = true,
                    });

                if (!result.Success)
                {
                    break;
                }
            }

            return result;
        }

        return new ShellCommandResult(true, string.Empty, string.Empty, 0);
    }

    private Task<ShellCommandResult> BuildContainer(string context, Dictionary<string, string>? env, Dictionary<string, string> buildArgs, string builder, bool? nonInteractive, List<string> tags, string fullDockerfilePath)
    {
        var buildArgumentBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerLiterals.BuildCommand, string.Empty, quoteValue: false);

        foreach (var tag in tags)
        {
            buildArgumentBuilder.AppendArgument(DockerLiterals.TagArgument, tag.ToLower(), allowDuplicates: true);
        }

        if (env is not null)
        {
            AddDockerBuildArgs(buildArgumentBuilder, env);
        }

        if (buildArgs is not null)
        {
            AddDockerBuildArgs(buildArgumentBuilder, buildArgs);
        }

        buildArgumentBuilder
            .AppendArgument(DockerLiterals.DockerFileArgument, fullDockerfilePath)
            .AppendArgument(context, string.Empty, quoteValue: false);

        return shellExecutionService.ExecuteCommand(new()
        {
            Command = builder,
            ArgumentsBuilder = buildArgumentBuilder,
            NonInteractive = nonInteractive.GetValueOrDefault(),
            ShowOutput = true,
        });
    }

    private async Task HandleBuildErrors(string command, ArgumentsBuilder argumentsBuilder, bool nonInteractive, string errors)
    {
        if (errors.Contains(DotNetSdkLiterals.DuplicateFileOutputError, StringComparison.OrdinalIgnoreCase))
        {
            await HandleDuplicateFilesInOutput(argumentsBuilder, nonInteractive);
            return;
        }

        if (errors.Contains(DotNetSdkLiterals.UnknownContainerRegistryAddress, StringComparison.OrdinalIgnoreCase))
        {
            console.MarkupLine($"[red bold]{DotNetSdkLiterals.UnknownContainerRegistryAddress}: Unknown container registry address, or container registry address not accessible.[/]");
            ActionCausesExitException.ExitNow(1013);
        }

        ActionCausesExitException.ExitNow();
    }

    private async Task HandleDuplicateFilesInOutput(ArgumentsBuilder argumentsBuilder, bool nonInteractive = false)
    {
        var shouldRetry = AskIfShouldRetryHandlingDuplicateFiles(nonInteractive);
        if (shouldRetry)
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.ErrorOnDuplicatePublishOutputFilesArgument, "false");

            await shellExecutionService.ExecuteCommand(new()
            {
                Command = DotNetSdkLiterals.DotNetCommand,
                ArgumentsBuilder = argumentsBuilder,
                NonInteractive = nonInteractive,
                OnFailed = HandleBuildErrors,
                ShowOutput = true,
            });
            return;
        }

        ActionCausesExitException.ExitNow();
    }

    private bool AskIfShouldRetryHandlingDuplicateFiles(bool nonInteractive)
    {
        if (nonInteractive)
        {
            return true;
        }

        return console.Confirm(
            "[red bold]Implicitly, dotnet publish does not allow duplicate filenames to be output to the artefact directory at build time.Would you like to retry the build explicitly allowing them?[/]");
    }

    private static void AddProjectPublishArguments(ArgumentsBuilder argumentsBuilder, string fullProjectPath, string? runtimeIdentifier)
    {
        var defaultRuntimeIdentifier = GetRuntimeIdentifier();

        argumentsBuilder
            .AppendArgument(DotNetSdkLiterals.PublishArgument, fullProjectPath)
            .AppendArgument(DotNetSdkLiterals.ContainerTargetArgument, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkLiterals.VerbosityArgument, DotNetSdkLiterals.DefaultVerbosity)
            .AppendArgument(DotNetSdkLiterals.NoLogoArgument, string.Empty, quoteValue: false);

        argumentsBuilder.AppendArgument(DotNetSdkLiterals.RuntimeIdentifierArgument, string.IsNullOrEmpty(runtimeIdentifier) ? defaultRuntimeIdentifier : runtimeIdentifier);
    }

    private static void AddContainerDetailsToArguments(ArgumentsBuilder argumentsBuilder,
        MsBuildContainerProperties containerDetails)
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

        if (containerDetails.ContainerImageTag is not null && containerDetails.ContainerImageTag.Contains(';'))
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.ContainerImageTagArguments,
                $"\\\"{containerDetails.ContainerImageTag}\\\"");
            return;
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
        var builderAvailable = shellExecutionService.IsCommandAvailable(builder);

        if (!builderAvailable.IsAvailable)
        {
            console.MarkupLine($"[red bold]{builder} is not available or found on your system.[/]");
            ActionCausesExitException.ExitNow();
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
        if (builderCheckResult.Success)
        {
            return;
        }

        var builderInfo = builderCheckResult.Output.TryParseAsJsonDocument();
        if (builderInfo == null || !builderInfo.RootElement.TryGetProperty("ServerErrors", out var errorProperty))
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
        ActionCausesExitException.ExitNow();
    }

    private static void CheckSuccess(ShellCommandResult result)
    {
        if (result.ExitCode != 0)
        {
            ActionCausesExitException.ExitNow(result.ExitCode);
        }
    }

    private static string GetRuntimeIdentifier()
    {
        var architecture = RuntimeInformation.OSArchitecture;

        return architecture == Architecture.Arm64 ? "linux-arm64" : "linux-x64";
    }
}
