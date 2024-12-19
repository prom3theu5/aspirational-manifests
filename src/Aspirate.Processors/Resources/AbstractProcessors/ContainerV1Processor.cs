using Aspirate.Shared.Models.AspireManifests.Components.V1.Container;

namespace Aspirate.Processors.Resources.AbstractProcessors;

public class ContainerV1Processor(
    IFileSystem fileSystem,
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IContainerCompositionService containerCompositionService,
    IContainerDetailsService containerDetailsService,
    IManifestWriter manifestWriter)
    : BaseResourceProcessor(fileSystem, console, manifestWriter)
{

    public override string ResourceType => AspireComponentLiterals.ContainerV1;

    public override Resource? Deserialize(ref Utf8JsonReader reader)
        => JsonSerializer.Deserialize<ContainerV1Resource>(ref reader);

    public override ComposeService CreateComposeEntry(CreateComposeEntryOptions options)
    {
        var response = new ComposeService();

        var container = options.Resource.Value as ContainerV1Resource;

        var service = Builder.MakeService(options.Resource.Key)
                             .WithBuild(builder =>
                             {
                                 builder.WithContext(container.Build.Context);
                                 builder.WithDockerfile(container.Build.Dockerfile);

                                 // TODO: revisit this later once enough info regarding `container.v1` is gathered
                                 // builder.WithArguments(argBuilder =>
                                 // {
                                 // });
                             });

        if (container.Args is not null)
        {
            service.WithCommands(container.Args.ToArray());
        }

        var newService = service
                         .WithEnvironment(options.Resource.MapResourceToEnvVars(options.WithDashboard))
                         .WithContainerName(options.Resource.Key);

        if (!string.IsNullOrEmpty(container.Entrypoint))
        {
            newService = newService.WithCommands(container.Entrypoint);
        }

        response.Service = newService.WithRestartPolicy(ERestartMode.UnlessStopped)
                                     .WithVolumes(options.Resource.MapComposeVolumes())
                                     .WithPortMappings(options.Resource.MapBindingsToPorts().MapPortsToDockerComposePorts())
                                     .Build();

        return response;
    }

    public override List<object> CreateKubernetesObjects(CreateKubernetesObjectsOptions options)
    {
        // TODO: implement k8s exporter
        return base.CreateKubernetesObjects(options);
    }

    public override Task<bool> CreateManifests(CreateManifestsOptions options)
    {
        // TODO: implement manifests exporter
        return base.CreateManifests(options);
    }
}
