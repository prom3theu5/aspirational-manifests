using Volume = Aspirate.DockerCompose.Models.Volume;

namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateDockerComposeManifestAction(IServiceProvider serviceProvider, IFileSystem fileSystem) : BaseAction(serviceProvider)
{
    private int _servicePort = 10000;

    public override Task<bool> ExecuteAsync()
    {
        var outputFormat = OutputFormat.FromValue(CurrentState.OutputFormat);

        if (outputFormat == OutputFormat.Kustomize)
        {
            Logger.MarkupLine($"[red](!)[/] The output format '{CurrentState.OutputFormat}' is not supported for this action.");
            Logger.MarkupLine($"[red](!)[/] Please use the output format 'compose' instead.");
            ActionCausesExitException.ExitNow();
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
        var volumes = CreateVolumes(services);

        var composeFile = Builder.MakeCompose()
            .WithServices(services.ToArray())
            .WithVolumes(volumes.ToArray())
            .Build();

        var composeFileString = composeFile.Serialize();

        if (!fileSystem.Directory.Exists(AspirateLiterals.DefaultOutputPath))
        {
            fileSystem.Directory.CreateDirectory(AspirateLiterals.DefaultOutputPath);
        }

        fileSystem.File.WriteAllText(outputFile, composeFileString);
    }

    private static List<Volume> CreateVolumes(List<Service> services)
    {
        var volumes = new List<Volume>();

        foreach (var service in services)
        {
            if (service.Volumes is not null)
            {
                volumes.AddRange(service.Volumes.Select(volume => new Volume { Name = volume.Split(':')[0] }));
            }
        }

        return volumes;
    }

    private void ProcessIndividualComponent(KeyValuePair<string, Resource> resource, List<Service> services)
    {
        if (AspirateState.IsNotDeployable(resource.Value))
        {
            return;
        }

        var handler = Services.GetKeyedService<IResourceProcessor>(resource.Value.Type);

        if (handler is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unsupported.[/]");
            return;
        }

        var response = handler.CreateComposeEntry(resource);

        if (response.IsProject)
        {
            foreach (var port in response.Service.Ports)
            {
                port.Published = _servicePort;
                _servicePort++;
            }
        }

        services.Add(response.Service);
    }
}
