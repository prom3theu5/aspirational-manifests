using Volume = Aspirate.Shared.Models.AspireManifests.Components.Common.Volume;

namespace Aspirate.Shared.Extensions;

public static class ResourceExtensions
{
    public static Dictionary<string, string?> MapResourceToEnvVars(this KeyValuePair<string, Resource> resource, bool? withDashboard)
    {
        var environment = new Dictionary<string, string?>();

        if (resource.Value is not IResourceWithEnvironmentalVariables { Env: not null } resourceWithEnv)
        {
            return environment;
        }

        foreach (var entry in resourceWithEnv.Env.Where(entry => !string.IsNullOrEmpty(entry.Value)))
        {
            environment.Add(entry.Key, entry.Value);
        }

        if (withDashboard == true)
        {
            environment.TryAdd("OTEL_EXPORTER_OTLP_ENDPOINT", "http://aspire-dashboard:18889");
            environment.TryAdd("OTEL_SERVICE_NAME", resource.Key);
        }

        return environment;
    }

    public static List<Volume> KuberizeVolumeNames(this List<Volume> containerVolumes, KeyValuePair<string, Resource> resource)
    {
        if (containerVolumes.Count == 0)
        {
            return containerVolumes;
        }

        foreach (var volume in containerVolumes)
        {
            if (string.IsNullOrEmpty(volume.Name))
            {
                volume.Name = $"{resource.Key}-{volume.Target}".ToLowerInvariant();
            }

            volume.Name = volume.Name.Replace("/", "-").Replace(".", "-").Replace("--", "-").ToLowerInvariant();
        }

        return containerVolumes;
    }

    public static string[] MapComposeVolumes(this KeyValuePair<string, Resource> resource, string outputPath)
    {
        var composeVolumes = new List<string>();

        if (resource.Value is IResourceWithVolumes resourceWithVolumes)
        {
            KuberizeVolumeNames(resourceWithVolumes.Volumes, resource); 

            composeVolumes.AddRange(resourceWithVolumes.Volumes.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(volume => $"{volume.Name}:{volume.Target}"));
        }

        if (resource.Value is IResourceWithBindMounts resourceWithBindMounts)
        {
            composeVolumes.AddRange(resourceWithBindMounts.BindMounts.Select(volume => $"./{Path.GetRelativePath(outputPath, volume.Source).Replace(Path.DirectorySeparatorChar, '/')}:{volume.Target}"));
        }

        return composeVolumes.ToArray();
    }

    public static List<Ports> MapBindingsToPorts(this KeyValuePair<string, Resource> resource)
    {
        if (resource.Value is not IResourceWithBinding resourceWithBinding)
        {
            return [];
        }

        return resourceWithBinding.Bindings?.Select(b => new Ports { Name = b.Key, InternalPort = b.Value.TargetPort.GetValueOrDefault(), ExternalPort = b.Value.Port.GetValueOrDefault() }).ToList() ?? [];
    }

    public static Port[] MapPortsToDockerComposePorts(this List<Ports> ports) =>
        ports.Select(x=> new Port
        {
            Target = x.InternalPort,
            Published = x.ExternalPort != 0 ? x.ExternalPort : x.InternalPort,
        }).ToArray();

    public static void EnsureBindingsHavePorts(this Dictionary<string, Resource> resources)
    {
        foreach (var resource in resources.Where(x=>x.Value is IResourceWithBinding {Bindings: not null}))
        {
            var bindingResource = resource.Value as IResourceWithBinding;

            foreach (var binding in bindingResource.Bindings)
            {
                if (binding.Key.Equals("http", StringComparison.OrdinalIgnoreCase) && binding.Value.TargetPort is 0 or null)
                {
                    binding.Value.TargetPort = 8080;
                }

                if (binding.Key.Equals("https", StringComparison.OrdinalIgnoreCase) && binding.Value.TargetPort is 0 or null)
                {
                    binding.Value.TargetPort = 8443;
                }
            }
        }
    }

    public static List<BindMount> NormalizeBindMountSource(this List<BindMount>? bindMounts, IFileSystem fileSystem)
    {
        if (bindMounts == null || !bindMounts.Any())
        {
            return [];
        }

        return bindMounts.Select(b => new BindMount
        {
            Source = fileSystem.GetFullPath(b.Source),
            Target = b.Target,
            ReadOnly = b.ReadOnly
        }).ToList();
    }
}
