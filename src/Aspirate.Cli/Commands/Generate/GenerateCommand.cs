namespace Aspirate.Cli.Commands.Generate;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
public sealed class GenerateCommand(
    IManifestFileParserService manifestFileParserService,
    IAnsiConsole console,
    IAspirateConfigurationService configurationService,
    IKubeCtlService kubeCtlService,
    IFileSystem fileSystem,
    IServiceProvider serviceProvider) : AsyncCommand<GenerateInput>
{
    public const string CommandName = "generate";
    public const string CommandDescription = "Builds, pushes containers, generates aspire manifest and kustomize manifests.";
    private static bool IsDatabase(Resource resource) =>
        resource is PostgresDatabase;

    public override async Task<int> ExecuteAsync(CommandContext context, GenerateInput settings)
    {
        var aspirateSettings = configurationService.LoadConfigurationFile(settings.PathToAspireProjectFlag);

        if (aspirateSettings is not null)
        {
            console.LogLoadedConfigurationFile(fileSystem.GetFullPath(settings.PathToAspireProjectFlag));
        }

        var appManifestFilePath = await GenerateAspireManifest(settings.PathToAspireProjectFlag);
        var aspireManifest = manifestFileParserService.LoadAndParseAspireManifest(appManifestFilePath);
        var finalManifests = new Dictionary<string, Resource>();

        var componentsToProcess = manifestFileParserService.SelectManifestItemsToProcess(aspireManifest.Keys.ToList());

        var projectsToProcess = aspireManifest.Where(x => x.Value is Project && componentsToProcess.Contains(x.Key)).ToList();

        var projectProcessor = serviceProvider.GetRequiredKeyedService<IProcessor>(AspireLiterals.Project) as ProjectProcessor;

        await PopulateProjectContainerDetailsCache(projectsToProcess, projectProcessor, aspirateSettings);

        await BuildAndPushProjectContainers(projectsToProcess, projectProcessor);

        await GenerateManifests(settings, aspireManifest, componentsToProcess, finalManifests, aspirateSettings);

        console.LogCommandCompleted();

        return 0;
    }

    private async Task<string> GenerateAspireManifest(
        string appHostPath)
    {
        console.LogGeneratingAspireManifest();

        var compositionService = serviceProvider.GetRequiredService<IAspireManifestCompositionService>();

        var result = await compositionService.BuildManifestForProject(appHostPath);

        if (result.Success)
        {
            await console.LogCreatedManifestAtPath(result.FullPath);
            return result.FullPath;
        }

        console.LogFailedToGenerateAspireManifest(result.FullPath);
        throw new InvalidOperationException("Failed to generate Aspire Manifest.");
    }

    private async Task PopulateProjectContainerDetailsCache(IReadOnlyCollection<KeyValuePair<string, Resource>> projectsToProcess,
        ProjectProcessor? projectProcessor,
        AspirateSettings? aspirateSettings)
    {
        console.LogGatheringContainerDetailsFromProjects();

        foreach (var resource in projectsToProcess)
        {
            await projectProcessor.PopulateContainerDetailsCacheForProject(resource, aspirateSettings);
        }

        await console.LogGatheringContainerDetailsFromProjectsCompleted();
    }

    private async Task BuildAndPushProjectContainers(
        IReadOnlyCollection<KeyValuePair<string, Resource>> projectsToProcess,
        ProjectProcessor? projectProcessor)
    {
        console.LogBuildingAndPushingContainers();

        foreach (var resource in projectsToProcess)
        {
            await projectProcessor.BuildAndPushProjectContainer(resource);
        }

        await console.LogContainerCompositionCompleted();
    }

    private async Task GenerateManifests(GenerateInput settings,
        Dictionary<string, Resource> aspireManifest,
        ICollection<string> componentsToProcess,
        Dictionary<string, Resource> finalManifests,
        AspirateSettings? aspirateSettings)
    {
        console.LogGeneratingManifests();

        foreach (var resource in aspireManifest.Where(x => x.Value is not UnsupportedResource && componentsToProcess.Contains(x.Key)))
        {
            await ProcessIndividualResourceManifests(settings, resource, finalManifests, aspirateSettings);
        }

        var finalHandler = serviceProvider.GetRequiredKeyedService<IProcessor>(AspireLiterals.Final) as FinalProcessor;
        finalHandler.CreateFinalManifest(finalManifests, settings.OutputPathFlag, aspirateSettings);
    }

    private async Task ProcessIndividualResourceManifests(GenerateInput input,
        KeyValuePair<string, Resource> resource,
        Dictionary<string, Resource> finalManifests,
        AspirateSettings? aspirateSettings)
    {
        if (resource.Value.Type is null)
        {
            console.LogTypeUnknown(resource.Key);
            return;
        }

        var handler = serviceProvider.GetKeyedService<IProcessor>(resource.Value.Type);

        if (handler is null)
        {
            console.LogUnsupportedType(resource.Key);
            return;
        }

        var success = await handler.CreateManifests(resource, input.OutputPathFlag, aspirateSettings);

        if (success && !IsDatabase(resource.Value))
        {
            finalManifests.Add(resource.Key, resource.Value);
        }
    }
}
