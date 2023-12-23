namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateDockerComposeManifestAction(IServiceProvider serviceProvider, IFileSystem fileSystem) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        var outputFormat = OutputFormat.FromValue(CurrentState.OutputFormat);

        if (outputFormat == OutputFormat.Kustomize)
        {
            Logger.MarkupLine($"[red](!)[/] The output format '{CurrentState.OutputFormat}' is not supported for this action.");
            Logger.MarkupLine($"[red](!)[/] Please use the output format 'compose' instead.");
            throw new ActionCausesExitException(1);
        }

        var outputFile = Path.Combine(AspirateLiterals.DefaultOutputPath, "docker-compose.yml");

        Logger.MarkupLine($"\r\n[bold]Generating docker compose file: [blue]'{outputFile}'[/][/]\r\n");

        var services = new List<Service>();

        foreach (var resource in CurrentState.AllSelectedSupportedComponents)
        {
            ProcessIndividualComponent(resource, services);
        }

        WriteFile(services, outputFile);

        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputFile}[/]");

        return Task.FromResult(true);
    }

    private void WriteFile(List<Service> services, string outputFile)
    {
        var composeFile = Builder.MakeCompose()
            .WithServices(services.ToArray())
            .Build();

        var composeFileString = composeFile.Serialize();

        if (!fileSystem.Directory.Exists(AspirateLiterals.DefaultOutputPath))
        {
            fileSystem.Directory.CreateDirectory(AspirateLiterals.DefaultOutputPath);
        }

        fileSystem.File.WriteAllText(outputFile, composeFileString);
    }

    private void ProcessIndividualComponent(KeyValuePair<string, Resource> resource, List<Service> services)
    {
        if (resource.Value.Type is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unknown.[/]");
            return;
        }

        if (!AspirateState.IsNotDeployable(resource.Value))
        {
            return;
        }

        var handler = Services.GetKeyedService<IResourceProcessor>(resource.Value.Type);

        if (handler is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unsupported.[/]");
            return;
        }

        var service = handler.CreateComposeEntry(resource);

        if (service is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its compose entry could not be created.[/]");
            return;
        }

        services.Add(service);
    }
}
