using Aspirate.Processors.Resources.AbstractProcessors;
using Aspirate.Processors.Resources.Dockerfile;

namespace Aspirate.Commands.Actions.Containers;

public sealed class BuildAndPushContainersFromDockerfilesAction(
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Dockerfiles[/]");

        if (!HasSelectedDockerfileComponents())
        {
            return true;
        }

        HandleComposeOutputBuildSelectionForDockerfiles();

        var dockerfileProcessor = Services.GetRequiredKeyedService<IResourceProcessor>(AspireComponentLiterals.Dockerfile) as DockerfileProcessor;
        var containerV1Processor = Services.GetRequiredKeyedService<IResourceProcessor>(AspireComponentLiterals.ContainerV1) as ContainerV1Processor;

        CacheContainerDetails(dockerfileProcessor, containerV1Processor);

        if (CurrentState.SkipBuild == true)
        {
            Logger.MarkupLine("[bold]Skipping build and push action as requested.[/]");
            return true;
        }

        await PerformBuildAndPushes(dockerfileProcessor, containerV1Processor);

        return true;
    }

    private void HandleComposeOutputBuildSelectionForDockerfiles()
    {
        if (string.IsNullOrEmpty(CurrentState.OutputFormat) || OutputFormat.FromValue(CurrentState.OutputFormat) != OutputFormat.DockerCompose)
        {
            return;
        }

        if (CurrentState.ComposeBuilds?.Any() == false)
        {
            SelectComposeItemsToIncludeAsComposeBuilds();
        }

        if (CurrentState.ComposeBuilds?.Any() == true)
        {
            Logger.MarkupLine("[bold]Compose builds selected:[/]");
            foreach (var composeBuild in CurrentState.ComposeBuilds)
            {
                Logger.MarkupLine($"[blue] - {composeBuild}[/]");
            }
        }
    }

    private void CacheContainerDetails(DockerfileProcessor dockerfileProcessor, ContainerV1Processor containerV1Processor)
    {
        foreach (var resource in CurrentState.SelectedDockerfileComponents.Where(resource => CurrentState.ComposeBuilds?.Contains(resource.Key) != true))
        {
            SelectImageProcessor(resource.Value, dockerfileProcessor, containerV1Processor)
                .PopulateContainerImageCacheWithImage(resource, new()
                {
                    Registry = CurrentState.ContainerRegistry,
                    Prefix = CurrentState.ContainerRepositoryPrefix,
                    Tags = CurrentState.ContainerImageTags,
                });
        }
    }

    private IImageProcessor SelectImageProcessor(Resource resource, DockerfileProcessor dockerfileProcessor, ContainerV1Processor containerV1Processor) =>
        resource is DockerfileResource ? dockerfileProcessor :
        resource is ContainerV1Resource ? containerV1Processor :
            throw new InvalidOperationException($"Unexpected resource type {resource?.GetType().Name}");

    private void SelectComposeItemsToIncludeAsComposeBuilds()
    {
        if (CurrentState.NonInteractive)
        {
            return;
        }

        var dockerFileEntries = CurrentState.LoadedAspireManifestResources.Where(x => x.Value is DockerfileResource)
            .Select(x => x.Key).ToList();

        var selectedEntries = Logger.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title(
                        "Select [green]Dockerfiles[/] to include as compose built images (The compose file is responsible for building the image)")
                    .PageSize(10)
                    .Required(false)
                    .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a component, " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoiceGroup("Select all", dockerFileEntries))
            .ToArray();

        ProcessSelectedComponents(selectedEntries);
    }

    private void ProcessSelectedComponents(string[] selectedEntries)
    {
        if (selectedEntries.Length == 0)
        {
            CurrentState.ComposeBuilds = null;
            return;
        }

        CurrentState.ComposeBuilds ??= [];

        foreach (var selectedEntry in selectedEntries)
        {
            if (CurrentState.ComposeBuilds.Contains(selectedEntry))
            {
                continue;
            }

            CurrentState.ComposeBuilds.Add(selectedEntry);
        }
    }

    private async Task PerformBuildAndPushes(DockerfileProcessor dockerfileProcessor, ContainerV1Processor containerV1Processor)
    {
        foreach (var resource in CurrentState.SelectedDockerfileComponents.Where(resource => CurrentState.ComposeBuilds?.Contains(resource.Key) != true))
        {
            await SelectImageProcessor(resource.Value, dockerfileProcessor, containerV1Processor)
                .BuildAndPushContainerForDockerfile(resource, new()
                {
                    ContainerBuilder = CurrentState.ContainerBuilder.ToLower(),
                    ImageName = resource.Key,
                    Registry = CurrentState.ContainerRegistry,
                    Prefix = CurrentState.ContainerRepositoryPrefix,
                    Tags = CurrentState.ContainerImageTags
                }, CurrentState.NonInteractive);
        }
    }

    private bool HasSelectedDockerfileComponents()
    {
        if (CurrentState.SelectedDockerfileComponents.Count > 0)
        {
            return true;
        }

        Logger.MarkupLine("[bold]No Dockerfile components selected. Skipping build and publish action.[/]");
        return false;
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.NonInteractive || !HasSelectedDockerfileComponents() || OutputFormat.FromValue(CurrentState.OutputFormat) != OutputFormat.DockerCompose)
        {
            return;
        }

        if (CurrentState.ComposeBuilds?.Any() == true)
        {
            foreach (var composeBuild in CurrentState.ComposeBuilds)
            {
                if (!CurrentState.LoadedAspireManifestResources.ContainsKey(composeBuild))
                {
                    Logger.ValidationFailed($"The resource '{composeBuild}' is not found in the loaded manifest.");
                }
            }
        }
    }
}
