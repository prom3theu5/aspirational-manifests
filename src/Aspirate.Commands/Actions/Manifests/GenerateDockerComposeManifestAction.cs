using Aspirate.DockerCompose.Models.Services;

namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateDockerComposeManifestAction(IServiceProvider serviceProvider, IFileSystem fileSystem)
    : BaseAction(serviceProvider)
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
        var composeOverrideFile = Path.Combine(AspirateLiterals.DefaultOutputPath, "docker-compose.override.yml");

        Logger.MarkupLine($"\r\n[bold]Generating docker compose file: [blue]'{outputFile}'[/][/]\r\n");

        var services = new List<Service>();
        var composeOverride = new List<Service>();

        if (CurrentState.ComposeOverride)
        {
            // Add aspire dashboard by default
            var aspireDashboardService = new Service
            {
                Name = "aspire",
                Image = "mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.4",
                Ports = new List<Port>
                {
                    new Port
                    {
                        Published = 18888,
                        Target = 18888
                    },
                    new Port
                    {
                        Target = 18889,
                        Published = 18889
                    }
                }
            };
            composeOverride.Add(aspireDashboardService);
        }

        foreach (var resource in CurrentState.AllSelectedSupportedComponents)
        {
            ProcessIndividualComponent(resource, services, composeOverride);
        }

        WriteFile(services, outputFile);
        if (CurrentState.ComposeOverride)
        {
            WriteFile(composeOverride, composeOverrideFile);
        }

        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputFile}[/]");

        return Task.FromResult(true);
    }

    private void WriteFile(List<Service> services, string outputFile)
    {
        var composeBuilder = Builder.MakeCompose()
            .WithServices(services.ToArray());

        var composeFile = composeBuilder.Build();

        var composeFileString = composeFile.Serialize();

        if (!fileSystem.Directory.Exists(AspirateLiterals.DefaultOutputPath))
        {
            fileSystem.Directory.CreateDirectory(AspirateLiterals.DefaultOutputPath);
        }

        fileSystem.File.WriteAllText(outputFile, composeFileString);
    }

    private void ProcessIndividualComponent(KeyValuePair<string, Resource> resource, List<Service> services,
        List<Service> composeOverride)
    {
        if (resource.Value.Type is null || AspirateState.IsNotDeployable(resource.Value) ||
            Services.GetKeyedService<IResourceProcessor>(resource.Value.Type) is not { } handler)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unknown or unsupported.[/]");
            return;
        }

        var response = handler.CreateComposeEntry(resource);

        if (response.IsProject)
        {
            response.Service.Ports.ForEach(port => port.Published = _servicePort++);
        }

        var serviceToAdd = CurrentState.ComposeOverride ? CreateNewService(response.Service) : response.Service;
        services.Add(serviceToAdd);

        if (CurrentState.ComposeOverride)
        {
            // Add reference to aspire OTLP collector
            response.Service.Environment.Add("OTEL_EXPORTER_OTLP_ENDPOINT", "http://aspire:18889");
            composeOverride.Add(response.Service);
        }
    }

    private Service CreateNewService(Service originalService) =>
        new()
        {
            Name = originalService.Name,
            Image = originalService.Image,
            Environment = originalService.Environment,
            ContainerName = originalService.ContainerName,
            Restart = originalService.Restart,
            Ports = new List<Port>(),
            Expose = new List<string>(),
            ExtraHosts = new List<string>(),
        };
}
