using Valleysoft.DockerCredsProvider;
using Aspirate.Shared.Models.ContainerRegistry;

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
        string? runtimeIdentifier = null,
        bool verifyImageAge = false,
        string? privateRegistryUsername = null,
        string? privateRegistryPassword = null)
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

        var publishStartTime = DateTime.UtcNow;

        await shellExecutionService.ExecuteCommand(new()
        {
            Command = DotNetSdkLiterals.DotNetCommand,
            ArgumentsBuilder = argumentsBuilder,
            NonInteractive = nonInteractive,
            OnFailed = HandleBuildErrors,
            ShowOutput = true,
        });

        if (verifyImageAge)
        {
            if (!string.IsNullOrEmpty(containerDetails.ContainerRegistry))
            {
                await VerifyRegistryImageAgeAsync(containerDetails, publishStartTime, privateRegistryUsername, privateRegistryPassword);
            }
            else
            {
                await VerifyLocalImageAgeAsync(containerDetails, options, nonInteractive, publishStartTime);
            }
        }

        return true;
    }

    public async Task<bool> BuildAndPushContainerForDockerfile(DockerfileResource dockerfileResource, ContainerOptions options, bool? nonInteractive = false)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        await CheckIfBuilderIsRunning(options.ContainerBuilder);

        var fullDockerfilePath = filesystem.GetFullPath(dockerfileResource.Path);

        var fullImages = options.ToImageNames(dockerfileResource.Name);

        var result = await BuildContainer(dockerfileResource, options.ContainerBuilder, nonInteractive, fullImages, fullDockerfilePath);

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

    private Task<ShellCommandResult> BuildContainer(DockerfileResource dockerfileResource, string builder, bool? nonInteractive, List<string> tags, string fullDockerfilePath)
    {
        var buildArgumentBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerLiterals.BuildCommand, string.Empty, quoteValue: false);

        foreach (var tag in tags)
        {
            buildArgumentBuilder.AppendArgument(DockerLiterals.TagArgument, tag.ToLower(), allowDuplicates: true);
        }

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

    private async Task VerifyLocalImageAgeAsync(MsBuildContainerProperties containerDetails, ContainerOptions options, bool? nonInteractive, DateTime publishStartTime)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        await CheckIfBuilderIsRunning(options.ContainerBuilder);

        console.MarkupLine($"Verifying age of [blue]{containerDetails.ContainerRepository}:{containerDetails.ContainerImageTag}[/]");

        var inspectCreatedArgumentBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument(DockerLiterals.InspectCommand, string.Empty, quoteValue: false)
            .AppendArgument(DockerLiterals.FormatArgument, "{{ .Created }}", quoteValue: true)
            .AppendArgument($"{containerDetails.ContainerRepository}:{containerDetails.ContainerImageTag}", string.Empty, quoteValue: false);

        var inspectCreatedResult = await shellExecutionService.ExecuteCommand(new()
        {
            Command = options.ContainerBuilder,
            ArgumentsBuilder = inspectCreatedArgumentBuilder,
            NonInteractive = nonInteractive.GetValueOrDefault(),
            ShowOutput = false,
        });

        if (inspectCreatedResult.Success)
        {
            return;
        }

        var created = DateTimeOffset.Parse(inspectCreatedResult.Output.Trim());

        if (created < publishStartTime)
        {
            console.MarkupLine($"[red bold]Local image [blue]'{containerDetails.ContainerRepository}:{containerDetails.ContainerImageTag}'[/] is out of date[/]");
            ActionCausesExitException.ExitNow(5016);
        }
    }

    private async Task VerifyRegistryImageAgeAsync(MsBuildContainerProperties containerDetails, DateTime publishStartTime, string? privateRegistryUsername, string? privateRegistryPassword)
    {
        console.MarkupLine($"Verifying age of [blue]{containerDetails.ContainerRepository}:{containerDetails.ContainerImageTag}[/] on registry [blue]{containerDetails.ContainerRegistry}[/]");

        // Credentials passed directly to Aspirate have priority, so we only check the
        // docker credentials if no values are explicitly passed in.
        if (privateRegistryUsername is null && privateRegistryPassword is null)
        {
            try
            {
                // To retrieve the docker credentials, we use the Valleysoft.DockerCredsProvider
                // package. This is what is used by the dotnet SDK so we can expect some degree
                // of functional by using it ourselves.
                var creds = await CredsProvider.GetCredentialsAsync(containerDetails.ContainerRegistry);

                // Only password supported currently.
                if (creds.Password is not null)
                {
                    console.MarkupLine($"Using docker credentials for user [blue]{creds.Username}[/]");
                    privateRegistryUsername = creds.Username;
                    privateRegistryPassword = creds.Password;
                }
            }
            catch (CredsNotFoundException)
            {
                // Unfortunate to use exception handling for regular control flow, but that's
                // the edesign of this library. No need to log, this can be swallowed.
            }
            catch (Exception e)
            {
                // However, we probably want to log errors otherwise. In any case, errors on
                // this path aren't necessarily fatal, so don't give up.
                console.MarkupLine($"[yellow bold]Error fetching docker credentials: {e.Message}[/]");
            }
        }

        ContainerRegistryV2Client registryClient;
        RegistryCatalogV2 registryCatalog;

        try
        {
            // At this point, we don't want any failures to be blocking. The intent of this
            // feature is to fail when the PublishContainer task is silently skipped by the
            // dotnet SDK. Unfortuantely, querying a container registry for creation date
            // (our means of confirming successful publish) is complicated by the different
            // types of registries and their behaviors.
            //
            // The current implementation of ContainerRegistryV2Client has been tested against
            // the official Docker registry 2.8.3 image, as well as Azure ACR. It DOES NOT
            // work with the docker.io (no basic auth and lack of catalog support). Other
            // registries in the ecosystem may fail as well.
            //
            // Unfortunately, MS has not made Microsoft.NET.Build.Containers.Registry public.
            // However, reviewing the implementation reveals the effort necessary to support
            // the various registries present in the ecosystem.
            //
            // Given all of this, we're only throwing a warning here. This gives users of the
            // --verify-image-age flag notice that they may want to avoid it. It is worth
            // noting this gap in functionality is also why image age verification is off by
            // default.
            registryClient = await ContainerRegistryV2Client.ConnectAsync(
                containerDetails.ContainerRegistry,
                privateRegistryUsername,
                privateRegistryPassword);

            registryCatalog = await registryClient.GetCatalogAsync();
        }
        catch (Exception e)
        {
            console.MarkupLine($"[yellow bold]Error querying container repository [blue]'{containerDetails.ContainerRepository}'[/]:[/]");

            IEnumerable<Exception> exceptions = e is AggregateException ae ?
                ae.Flatten().InnerExceptions :
                [e];

            foreach (var e2 in exceptions)
            {
                console.MarkupLine($"[yellow]{e2.Message}[/]");
            }

            return;
        }

        // Any failure at this point is considered fatal. If any unhandled exceptions arise, it is
        // expected the user has some understanding of what's going on given that the functionality
        // is opt-in.

        // Here's the first check. If no repository exists that matches ours, the PublishContainer
        // task was probably skipped by the dotnet SDK.
        if (!registryCatalog.Repositories.Contains(containerDetails.ContainerRepository))
        {

            console.MarkupLine($"[red bold]Could not find container repository [blue]'{containerDetails.ContainerRepository}'[/] in registry[/]");
            ActionCausesExitException.ExitNow(5013);
        }

        var tagList = await registryClient.GetTagsAsync(containerDetails.ContainerRepository);

        // Here's the second check. If no tag matches ours, PublishContainer was probably skipped.
        if (!tagList.Tags.Contains(containerDetails.ContainerImageTag))
        {
            console.MarkupLine($"[red bold]Could not find container image tag [blue]'{containerDetails.ContainerImageTag}'[/] for repository [blue]'{containerDetails.ContainerRepository}'[/] in registry[/]");
            ActionCausesExitException.ExitNow(5014);
        }

        var imageManifestList = await registryClient.GetManifestAsync(
            containerDetails.ContainerRepository,
            containerDetails.ContainerImageTag);

        var image = await registryClient.GetDockerImageJsonBlobAsync(
            containerDetails.ContainerRepository,
            imageManifestList.Config.Digest);

        // Here's the third and final check. If a docker image exists for our expected repository
        // and tag, but the creation date is before our attempted publish occurred, the
        // PublishContainer task was most likely skipped, and the image is stale.
        if (image.Created < publishStartTime)
        {
            console.MarkupLine($"[red bold]Registry image [blue]'{containerDetails.ContainerRepository}:{containerDetails.ContainerImageTag}'[/] is out of date[/]");
            ActionCausesExitException.ExitNow(5015);
        }
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
