namespace Aspirational.Manifests.Commands.EndToEnd;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
[Description("Convert Aspire Manifests to Kustomize Manifests")]
public sealed class EndToEndCommand : OaktonCommand<EndToEndInput>
{
    private readonly ManifestFileParserService _manifestFileParserService = new();

    /// <summary>
    /// Execute the command
    /// </summary>
    /// <param name="input">The input parameters to use.</param>
    /// <returns>a boolean representing the success state of the run.</returns>
    public override bool Execute(EndToEndInput input)
    {
        ValidateInput(input);

        var aspireManifest = _manifestFileParserService.LoadAndParseAspireManifest(input.PathToAspireManifestFlag);
        var finalManifests = new Dictionary<string, Resource>();

        foreach (var resource in aspireManifest.Where(x => x.Value is not UnsupportedResource))
        {
            ProcessIndividualResources(input, resource, finalManifests);
        }

        HandlerMapping.ResourceTypeToHandlerMap.TryGetValue(AspireResourceLiterals.Final, out var finalHandler);
        finalHandler.CreateFinalManifest(finalManifests, input.OutputPathFlag);

        return true;
    }

    private static void ProcessIndividualResources(EndToEndInput input, KeyValuePair<string, Resource> resource, Dictionary<string, Resource> finalManifests)
    {
        if (resource.Value.Type is null)
        {
            AnsiConsole.MarkupLine($"Skipping resource [green]'{resource.Key}'[/] as its type is unknown.");
            return;
        }

        HandlerMapping.ResourceTypeToHandlerMap.TryGetValue(resource.Value.Type, out var handler);

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

    private static void ValidateInput(EndToEndInput input)
    {
        if (string.IsNullOrWhiteSpace(input.PathToAspireManifestFlag))
        {
            throw new InvalidOperationException("The path to the aspire manifest file is required.");
        }

        if (string.IsNullOrWhiteSpace(input.OutputPathFlag))
        {
            throw new InvalidOperationException("The output path is required.");
        }
    }
}
