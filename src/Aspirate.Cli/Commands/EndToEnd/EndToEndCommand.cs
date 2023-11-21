namespace Aspirate.Cli.Commands.EndToEnd;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
public partial class EndToEndCommand(IServiceProvider serviceProvider, ILogger<EndToEndCommand> logger) : AsyncCommand<EndToEndInput>
{
    public static void RegisterEndToEndCommand(IConfigurator config) =>
        config.AddCommand<EndToEndCommand>("endtoend")
            .WithAlias("e2e")
            .WithDescription("Fully convert an aspire manifest to kustomize manifests.")
            .WithExample(["endtoend", "-m", "./Example/aspire-manifest.json", "-o", "./output"]);

    public override async Task<int> ExecuteAsync(CommandContext context, EndToEndInput input)
    {
        var shouldSelectivelyProcessInfrastructure = AskIfShouldSelectivelyProcessInfrastructure();

        using var scope = serviceProvider.CreateScope();

        var manifestFileParserService = scope.ServiceProvider.GetRequiredService<IManifestFileParserService>();
        var aspireManifest = manifestFileParserService.LoadAndParseAspireManifest(input.PathToAspireManifestFlag);
        var finalManifests = new Dictionary<string, Resource>();

        foreach (var resource in aspireManifest.Where(x => x.Value is not UnsupportedResource))
        {
            if (IsInfrastructure(resource.Value) && shouldSelectivelyProcessInfrastructure && !AskIfShouldDeployInfrastructure(resource.Value.Type))
            {
                continue;
            }

            await ProcessIndividualResources(scope, input, resource, finalManifests);
        }

        var finalHandler = scope.ServiceProvider.GetRequiredKeyedService<IProcessor>(AspireResourceLiterals.Final);
        finalHandler.CreateFinalManifest(finalManifests, input.OutputPathFlag);

        return 0;
    }

    public override ValidationResult Validate(CommandContext context, EndToEndInput input)
    {
        if (string.IsNullOrWhiteSpace(input.PathToAspireManifestFlag))
        {
            return ValidationResult.Error("The path to the aspire manifest file is required.");
        }

        if (string.IsNullOrWhiteSpace(input.OutputPathFlag))
        {
            return ValidationResult.Error("The output path is required.");
        }

        return ValidationResult.Success();
    }

    private async Task ProcessIndividualResources(
        IServiceScope scope,
        EndToEndInput input,
        KeyValuePair<string, Resource> resource,
        IDictionary<string, Resource> finalManifests)
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));

        if (resource.Value.Type is null)
        {
            LogTypeUnknown(logger, resource.Key);
            return;
        }

        var handler = scope.ServiceProvider.GetKeyedService<IProcessor>(resource.Value.Type);

        if (handler is null)
        {
            LogUnsupportedType(logger, resource.Key);
            return;
        }

        var success = await handler.CreateManifests(resource, input.OutputPathFlag);

        if (success && !IsDatabase(resource.Value))
        {
            finalManifests.Add(resource.Key, resource.Value);
        }
    }

    private static bool IsInfrastructure(Resource resource) =>
        resource is PostgresServer or Redis or PostgresDatabase;

    private static bool IsDatabase(Resource resource) =>
        resource is PostgresDatabase;

    private static bool AskIfShouldDeployInfrastructure(string typeName)
    {
        AnsiConsole.MarkupLine($"[yellow]Detected Infrastructure Resource [green]'{typeName}'[/].[/]");

        return AnsiConsole.Confirm("Do you wish to also deploy this?");
    }

    private static bool AskIfShouldSelectivelyProcessInfrastructure()
    {
        AnsiConsole.MarkupLine("[blue]Do you wish to selectively process Infrastructure?[/]");

        return AnsiConsole.Confirm("(Selecting No will generate all manifests)?");
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Skipping resource '{ResourceName}' as its type is unknown.")]
    static partial void LogTypeUnknown(ILogger logger, string resourceName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Skipping resource '{ResourceName}' as its type is not supported.")]
    static partial void LogUnsupportedType(ILogger logger, string resourceName);
}
