using Aspirate.Shared.Interfaces.Processors;
using Volume = DockerComposeBuilder.Model.Volume;

namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateDockerComposeManifestAction(IServiceProvider serviceProvider, IFileSystem fileSystem) : BaseAction(serviceProvider)
{
    private int _servicePort = 10000;

    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Docker Compose generation[/]");

        var outputFormat = OutputFormat.FromValue(CurrentState.OutputFormat);

        if (outputFormat == OutputFormat.Kustomize)
        {
            Logger.MarkupLine($"[red](!)[/] The output format '{CurrentState.OutputFormat}' is not supported for this action.");
            Logger.MarkupLine($"[red](!)[/] Please use the output format 'compose' instead.");
            ActionCausesExitException.ExitNow();
        }

        var outputFile = Path.Combine(AspirateLiterals.DefaultOutputPath, "docker-compose.yaml");

        Logger.MarkupLine($"[bold]Generating docker compose file: [blue]'{outputFile}'[/][/]");

        var services = new List<Service>();

        foreach (var resource in CurrentState.AllSelectedSupportedComponents)
        {
            ProcessIndividualComponent(resource, services);
        }

        if (CurrentState.IncludeDashboard.GetValueOrDefault())
        {
            AddAspireDashboardToCompose(services);
        }

        WriteFile(services, outputFile);

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputFile}[/]");

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

        var response = handler.CreateComposeEntry(new()
        {
            Resource = resource,
            WithDashboard = CurrentState.IncludeDashboard,
            ComposeBuilds = CurrentState.ComposeBuilds?.Any(x=> x == resource.Key) ?? false,
        });

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

    private static void AddAspireDashboardToCompose(List<Service> services)
    {
        var environment = new Dictionary<string, string?> { { "DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS", "true" } };

        var ports = new List<Port>
        {
            new() { Published = 18888, Target = 18888 }
        };

        var aspireDashboard = Builder.MakeService("aspire-dashboard")
            .WithImage("mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6")
            .WithEnvironment(environment)
            .WithContainerName("aspire-dashboard")
            .WithRestartPolicy(ERestartMode.UnlessStopped)
            .WithPortMappings(ports.ToArray())
            .Build();

        services.Insert(0, aspireDashboard);
    }
}
