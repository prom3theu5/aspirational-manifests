using Volume = Aspirate.Shared.Models.AspireManifests.Components.V0.Volume;

namespace Aspirate.Shared.Models.Aspirate;

[ExcludeFromCodeCoverage]
public class KubernetesDeploymentData
{
    public string? Name {get; private set;}
    public string? Namespace {get; private set;}
    public Dictionary<string, string>? Env {get; private set;}
    public Dictionary<string, string>? Secrets {get; private set;}
    public Dictionary<string, string>? Annotations {get; private set;}
    public List<Volume>? Volumes {get; private set;}
    public IReadOnlyCollection<string>? Manifests {get; private set;}
    public IReadOnlyCollection<string>? Args {get; private set;}
    public bool? IsService { get; private set; } = true;
    public bool? IsProject {get; private set;}
    public bool? WithPrivateRegistry { get; private set; } = false;
    public bool? WithDashboard { get; private set; } = false;
    public string? ContainerImage {get; private set;}
    public string? Entrypoint {get; private set;}
    public string? ImagePullPolicy {get; private set;}
    public List<Ports>? Ports {get; private set;}
    public string? ServiceType { get; private set; } = "ClusterIP";

    public KubernetesDeploymentData SetName(string name)
    {
        Name = name.ToLowerInvariant();
        return this;
    }

    public KubernetesDeploymentData SetNamespace(string? ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return this;
        }

        Namespace = ns.ToLowerInvariant();
        return this;
    }

    public KubernetesDeploymentData SetEnv(Dictionary<string, string> env)
    {
        Env = env.Where(x=>!string.IsNullOrEmpty(x.Value)).ToDictionary(x => x.Key, x => x.Value);
        return this;
    }

    public KubernetesDeploymentData SetSecrets(Dictionary<string, string> secrets)
    {
        Secrets = secrets;
        return this;
    }

    public KubernetesDeploymentData SetAnnotations(Dictionary<string, string> annotations)
    {
        Annotations = annotations;
        return this;
    }

    public KubernetesDeploymentData SetManifests(IReadOnlyCollection<string> manifests)
    {
        Manifests = manifests;
        return this;
    }

    public KubernetesDeploymentData SetIsService(bool service)
    {
        IsService = service;
        return this;
    }

    public KubernetesDeploymentData SetArgs(IReadOnlyCollection<string>? args)
    {
        Args = args;
        return this;
    }

    public KubernetesDeploymentData SetIsProject(bool project)
    {
        IsProject = project;
        return this;
    }

    public KubernetesDeploymentData SetEntrypoint(string? entrypoint)
    {
        Entrypoint = entrypoint;
        return this;
    }

    public KubernetesDeploymentData SetVolumes(List<Volume>? volumes)
    {
        Volumes = volumes ?? [];
        return this;
    }

    public KubernetesDeploymentData SetWithPrivateRegistry(bool isPrivateRegistry)
    {
        WithPrivateRegistry = isPrivateRegistry;
        return this;
    }

    public KubernetesDeploymentData SetContainerImage(string containerImage)
    {
        ContainerImage = containerImage.ToLowerInvariant();
        return this;
    }

    public KubernetesDeploymentData SetImagePullPolicy(string? imagePullPolicy)
    {
        ImagePullPolicy = imagePullPolicy ?? "IfNotPresent";
        return this;
    }

    public KubernetesDeploymentData SetWithDashboard(bool? withDashboard)
    {
        WithDashboard = withDashboard ?? false;
        return this;
    }

    public KubernetesDeploymentData SetPorts(List<Ports>? ports)
    {
        Ports = ports ?? [];
        return this;
    }

    public KubernetesDeploymentData SetServiceType(string? serviceType)
    {
        ServiceType = serviceType ?? "ClusterIP";
        return this;
    }

    public KubernetesDeploymentData SetSecretsFromSecretState(KeyValuePair<string, Resource> resource, ISecretProvider secretProvider, bool? disableSecrets = false)
    {
        if (disableSecrets == true)
        {
            return this;
        }

        if (!secretProvider.ResourceExists(resource.Key))
        {
            return this;
        }

        for (int i = 0; i < Secrets.Count; i++)
        {
            var secret = Secrets.ElementAt(i);

            if (!secretProvider.SecretExists(resource.Key, secret.Key))
            {
                continue;
            }

            var encryptedSecret = secretProvider.GetSecret(resource.Key, secret.Key);

            Secrets[secret.Key] = encryptedSecret;
        }

        return this;
    }

    public bool HasPorts => Ports?.Any() == true;
    public bool HasVolumes => Volumes?.Any() == true;
    public bool HasAnySecrets => Secrets?.Any() == true;
    public bool HasAnyAnnotations => Annotations?.Any() == true;
    public bool HasArgs => Args?.Any() == true;
    public bool WithNamespace => !string.IsNullOrWhiteSpace(Namespace);

    public KubernetesDeploymentData Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Name must be set.");
        }

        return this;
    }
}
