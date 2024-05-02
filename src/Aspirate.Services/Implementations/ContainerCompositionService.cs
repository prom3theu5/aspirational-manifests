using System.Runtime.InteropServices;
using Aspirate.Shared.Outputs;

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

        await AddProjectPublishArguments(argumentsBuilder, fullProjectPath, runtimeIdentifier);
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

    public async Task<bool> BuildAndPushContainerForDockerfile(DockerfileResource dockerfileResource, ContainerOptions options, bool? nonInteractive = false)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        await CheckIfBuilderIsRunning(options.ContainerBuilder);

        var fullDockerfilePath = filesystem.GetFullPath(dockerfileResource.Path);

        var fullImage = options.ToImageName(dockerfileResource.Name);

        var result = await BuildContainer(dockerfileResource, options.ContainerBuilder, nonInteractive, fullImage, fullDockerfilePath);

        CheckSuccess(result);

        result = await PushContainer(options.ContainerBuilder, options.Registry, fullImage, nonInteractive);

        CheckSuccess(result);

        return true;
    }

    private async Task<ShellCommandResult> PushContainer(string builder, string? registry, string fullImage, bool? nonInteractive)
    {
        if (!string.IsNullOrEmpty(registry))
        {
            var pushArgumentBuilder = ArgumentsBuilder
                .Create()
                .AppendArgument(DockerLiterals.PushCommand, string.Empty, quoteValue: false)
                .AppendArgument(fullImage.ToLower(), string.Empty, quoteValue: false);

            return await shellExecutionService.ExecuteCommand(
                new()
                {
                    Command = builder,
                    ArgumentsBuilder = pushArgumentBuilder,
                    NonInteractive = nonInteractive.GetValueOrDefault(),
                    ShowOutput = true,
                });
        }

        return new ShellCommandResult(true, string.Empty, string.Empty, 0);
    }

    private Task<ShellCommandResult> BuildContainer(DockerfileResource dockerfileResource, string builder, bool? nonInteractive, string tag, string fullDockerfilePath)
    {
        var buildArgumentBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerLiterals.BuildCommand, string.Empty, quoteValue: false)
            .AppendArgument(DockerLiterals.TagArgument, tag.ToLower());


        if (dockerfileResource.Env is not null)
        {
            AddDockerBuildArgs(buildArgumentBuilder, dockerfileResource.Env);
        }

        if (dockerfileResource.BuildArgs is not null)
        {
            AddDockerBuildArgs(buildArgumentBuilder, dockerfileResource.BuildArgs);
        }

        buildArgumentBuilder
            .AppendArgument(DockerLiterals.DockerFileArgument, fullDockerfilePath)
            .AppendArgument(dockerfileResource.Context, string.Empty, quoteValue: false);

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

    private async Task AddProjectPublishArguments(ArgumentsBuilder argumentsBuilder, string fullProjectPath, string? runtimeIdentifier)
    {
        var defaultRuntimeIdentifier = GetRuntimeIdentifier();

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
