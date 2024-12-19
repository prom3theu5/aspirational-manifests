namespace Aspirate.Commands.Actions.Manifests;

public class ApplyDaprAnnotationsAction(IServiceProvider serviceProvider, IAnsiConsole console) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling DAPR Components[/]");

        var daprSystemComponents = CurrentState.LoadedAspireManifestResources.Where(
            x => x.Value.Type.Equals(AspireComponentLiterals.DaprSystem, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (daprSystemComponents.Count == 0)
        {
            console.WriteLine("No Dapr components selected, skipping Dapr annotations.");
            return Task.FromResult(true);
        }

        foreach (var daprSystemComponent in daprSystemComponents)
        {
            var resource = daprSystemComponent.Value as DaprResource;

            var serviceForSidecar = CurrentState.LoadedAspireManifestResources[resource.Metadata.Application];

            ApplyDaprAnnotationsToTargetService(serviceForSidecar, resource);
        }

        return Task.FromResult(true);
    }

    private static void ApplyDaprAnnotationsToTargetService(Resource serviceForSidecar, DaprResource resource)
    {
        if (serviceForSidecar is not IResourceWithAnnotations service)
        {
            return;
        }

        service.Annotations ??= [];

        service.Annotations.Add("dapr.io/enabled", "'true'");
        service.Annotations.Add("dapr.io/config", "tracing");
        service.Annotations.Add("dapr.io/app-id", resource.Metadata.AppId);
        service.Annotations.Add("dapr.io/enable-api-logging", "'true'");

        HandleContainerPort(serviceForSidecar);
    }

    private static void HandleContainerPort(Resource serviceForSidecar)
    {
        if (serviceForSidecar is not ContainerV0Resource container)
        {
            return;
        }

        if (!container.Bindings.TryGetValue("tcp", out var binding))
        {
            return;
        }

        container.Annotations.Add("dapr.io/app-port", binding.TargetPort.ToString());
    }
}
