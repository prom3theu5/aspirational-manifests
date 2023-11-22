namespace Aspirate.Cli.Commands.EndToEnd;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
public class EndToEndCommand(IServiceProvider serviceProvider) : AsyncCommand<EndToEndInput>
{
    public const string EndToEndCommandName = "endtoend";
    public const string EndToEndDescription = "Builds, pushes containers, generates aspire manifest and kustomize manifests.";

    public override async Task<int> ExecuteAsync(CommandContext context, EndToEndInput settings)
    {
        using var scope = serviceProvider.CreateScope();

        var appManifestFilePath = await GenerateAspireManifest(settings.PathToAspireProjectFlag, scope);

        var manifestFileParserService = scope.ServiceProvider.GetRequiredService<IManifestFileParserService>();
        var aspireManifest = manifestFileParserService.LoadAndParseAspireManifest(appManifestFilePath);
        var finalManifests = new Dictionary<string, Resource>();

        var componentsToProcess = SelectManifestItemsToProcess(aspireManifest.Keys.ToList());

        await BuildAndPushProjectContainers(aspireManifest, componentsToProcess, scope);

        await GenerateManifests(settings, aspireManifest, componentsToProcess, scope, finalManifests);

        LogCommandCompleted();

        return 0;
    }

    private static async Task<string> GenerateAspireManifest(
        string appHostPath,
        IServiceScope scope)
    {
        Console.Clear();

        LogGeneratingAspireManifest();

        var compositionService = scope.ServiceProvider.GetRequiredService<IAspireManifestCompositionService>();

        var result = await compositionService.BuildManifestForProject(appHostPath);

        if (result.Success)
        {
            await LogCreatedManifestAtPath(result.FullPath);
            return result.FullPath;
        }

        AnsiConsole.MarkupLine($"[red]Failed to generate Aspire Manifest at: {result.FullPath}[/]");
        throw new InvalidOperationException("Failed to generate Aspire Manifest.");
    }

    private static async Task LogCreatedManifestAtPath(string resultFullPath)
    {
        AnsiConsole.MarkupLine($"\t[green](✔) Done: [/] Created Aspire Manifest At Path: [blue]{resultFullPath}[/]");
        await Task.Delay(2000);
        Console.Clear();
    }

    private static async Task BuildAndPushProjectContainers(
        Dictionary<string, Resource> aspireManifest,
        ICollection<string> componentsToProcess,
        IServiceScope scope)
    {
        Console.Clear();

        LogBuildingAndPushingContainers();

        var handler = scope.ServiceProvider.GetRequiredKeyedService<IProcessor>(AspireLiterals.Project) as ProjectProcessor;

        foreach (var resource in aspireManifest.Where(x => x.Value is Project && componentsToProcess.Contains(x.Key)))
        {
            await handler.BuildAndPushProjectContainer(resource);
        }

        await LogContainerCompositionCompleted();
    }

    private static async Task GenerateManifests(EndToEndInput settings,
        Dictionary<string, Resource> aspireManifest,
        ICollection<string> componentsToProcess,
        IServiceScope scope,
        Dictionary<string, Resource> finalManifests)
    {
        Console.Clear();

        LogGeneratingManifests();

        foreach (var resource in aspireManifest.Where(x => x.Value is not UnsupportedResource && componentsToProcess.Contains(x.Key)))
        {
            await ProcessIndividualResourceManifests(scope, settings, resource, finalManifests);
        }

        var finalHandler = scope.ServiceProvider.GetRequiredKeyedService<IProcessor>(AspireLiterals.Final);
        finalHandler.CreateFinalManifest(finalManifests, settings.OutputPathFlag);
    }

    private static async Task ProcessIndividualResourceManifests(
        IServiceScope scope,
        EndToEndInput input,
        KeyValuePair<string, Resource> resource,
        Dictionary<string, Resource> finalManifests)
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));

        if (resource.Value.Type is null)
        {
            LogTypeUnknown(resource.Key);
            return;
        }

        var handler = scope.ServiceProvider.GetKeyedService<IProcessor>(resource.Value.Type);

        if (handler is null)
        {
            LogUnsupportedType(resource.Key);
            return;
        }

        var success = await handler.CreateManifests(resource, input.OutputPathFlag);

        if (success && !IsDatabase(resource.Value))
        {
            finalManifests.Add(resource.Key, resource.Value);
        }
    }

    private static void LogGeneratingManifests() =>
        AnsiConsole.MarkupLine("\r\n[bold]Generating kustomize manifests to run against your kubernetes cluster:[/]\r\n");

    private static void LogGeneratingAspireManifest() =>
        AnsiConsole.MarkupLine("\r\n[bold]Generating Aspire Manifest for supplied App Host:[/]\r\n");

    private static void LogBuildingAndPushingContainers() =>
        AnsiConsole.MarkupLine("\r\n[bold]Building all project resources, and pushing containers:[/]\r\n");

    private static bool IsDatabase(Resource resource) =>
        resource is PostgresDatabase;

    private static void LogTypeUnknown(string resourceName) =>
        AnsiConsole.MarkupLine($"[yellow]Skipping resource '{resourceName}' as its type is unknown.[/]");

    private static void LogUnsupportedType(string resourceName) =>
        AnsiConsole.MarkupLine($"[yellow]Skipping resource '{resourceName}' as its type is unsupported.[/]");

    private static Task LogGenerationComplete()
    {
        AnsiConsole.MarkupLine("\r\n[bold slowblink]Generation completed.[/]");

        return Task.Delay(2000);
    }

    private static Task LogContainerCompositionCompleted()
    {
        AnsiConsole.MarkupLine("\r\n[bold slowblink]Generation completed.[/]");

        return Task.Delay(2000);
    }

    private static void LogCommandCompleted() =>
        AnsiConsole.MarkupLine("\r\n[bold slowblink] 🚀 Execution Completed - Happy Deployment 😃[/]");

    private static List<string> SelectManifestItemsToProcess(IEnumerable<string> manifestItems) =>
        AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]components[/] to process from the loaded file")
                .PageSize(10)
                .Required()
                .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a component, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("All Components", manifestItems));

}
