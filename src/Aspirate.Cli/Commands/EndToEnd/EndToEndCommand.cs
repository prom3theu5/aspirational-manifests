namespace Aspirate.Cli.Commands.EndToEnd;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
public sealed class EndToEndCommand(IServiceProvider serviceProvider) : Command<EndToEndInput>
{
    public static void RegisterEndToEndCommand(IConfigurator config) =>
        config.AddCommand<EndToEndCommand>("endtoend")
            .WithAlias("e2e")
            .WithDescription("Fully convert an aspire manifest to kustomize manifests.")
            .WithExample(["endtoend", "-m", "./Example/aspire-manifest.json", "-o", "./output"]);

    public override int Execute([NotNull] CommandContext context, [NotNull] EndToEndInput input)
    {
        using var scope = serviceProvider.CreateScope();

        var _manifestFileParserService = scope.ServiceProvider.GetRequiredService<IManifestFileParserService>();
        var aspireManifest = _manifestFileParserService.LoadAndParseAspireManifest(input.PathToAspireManifestFlag);
        var finalManifests = new Dictionary<string, Resource>();

        foreach (var resource in aspireManifest.Where(x => x.Value is not UnsupportedResource))
        {
            ProcessIndividualResources(scope, input, resource, finalManifests);
        }

        var finalHandler = scope.ServiceProvider.GetRequiredKeyedService<IProcessor>(AspireResourceLiterals.Final);
        finalHandler.CreateFinalManifest(finalManifests, input.OutputPathFlag);

        return 0;
    }

    public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] EndToEndInput input)
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

    private static void ProcessIndividualResources(
        IServiceScope scope,
        EndToEndInput input,
        KeyValuePair<string, Resource> resource,
        Dictionary<string, Resource> finalManifests)
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));

        if (resource.Value.Type is null)
        {
            AnsiConsole.MarkupLine($"Skipping resource [green]'{resource.Key}'[/] as its type is unknown.");
            return;
        }

        var handler = scope.ServiceProvider.GetKeyedService<IProcessor>(resource.Value.Type);

        if (handler is null)
        {
            AnsiConsole.MarkupLine($"Skipping resource [green]'{resource.Key}'[/] as its type [green]'{resource.Value.Type}'[/] is not supported.");
            return;
        }

        var success = handler.CreateManifests(resource, input.OutputPathFlag);

        if (success)
        {
            finalManifests.Add(resource.Key, resource.Value);
        }
    }
}
