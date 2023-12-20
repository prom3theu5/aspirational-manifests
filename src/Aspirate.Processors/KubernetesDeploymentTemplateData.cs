namespace Aspirate.Processors;

[ExcludeFromCodeCoverage]
public class KubernetesDeploymentTemplateData
{
    public string? Name {get; private set;}
    public string? Namespace {get; private set;}
    public Dictionary<string, string>? Env {get; private set;}
    public Dictionary<string, string>? Secrets {get; private set;}
    public IReadOnlyCollection<string>? Manifests {get; private set;}
    public bool? IsService { get; private set; } = true;
    public bool? IsProject {get; private set;}
    public bool? IsContainer {get; private set;}
    public string? ContainerImage {get; private set;}
    public bool? IsDockerfile {get; private set;}
    public string? ImagePullPolicy {get; private set;}
    public List<Ports>? Ports {get; private set;}
    public string? ServiceType {get; private set;}

    public KubernetesDeploymentTemplateData SetName(string name)
    {
        Name = name.ToLowerInvariant();
        return this;
    }

    public KubernetesDeploymentTemplateData SetNamespace(string ns)
    {
        Namespace = ns.ToLowerInvariant();
        return this;
    }

    public KubernetesDeploymentTemplateData SetEnv(Dictionary<string, string> env)
    {
        Env = env;
        return this;
    }

    public KubernetesDeploymentTemplateData SetSecrets(Dictionary<string, string> secrets)
    {
        Secrets = secrets;
        return this;
    }

    public KubernetesDeploymentTemplateData SetManifests(IReadOnlyCollection<string> manifests)
    {
        Manifests = manifests;
        return this;
    }

    public KubernetesDeploymentTemplateData SetIsService(bool service)
    {
        IsService = service;
        return this;
    }

    public KubernetesDeploymentTemplateData SetIsProject(bool project)
    {
        IsProject = project;
        return this;
    }

    public KubernetesDeploymentTemplateData SetContainerImage(string containerImage)
    {
        ContainerImage = containerImage.ToLowerInvariant();
        return this;
    }

    public KubernetesDeploymentTemplateData SetIsDockerfile(bool dockerfile)
    {
        IsDockerfile = dockerfile;
        return this;
    }

    public KubernetesDeploymentTemplateData SetIsContainer(bool container)
    {
        IsContainer = container;
        return this;
    }

    public KubernetesDeploymentTemplateData SetImagePullPolicy(string imagePullPolicy)
    {
        ImagePullPolicy = imagePullPolicy ?? "IfNotPresent";
        return this;
    }

    public KubernetesDeploymentTemplateData SetPorts(List<Ports>? ports)
    {
        Ports = ports ?? [];
        return this;
    }

    public KubernetesDeploymentTemplateData SetServiceType(string? serviceType)
    {
        ServiceType = serviceType ?? "ClusterIP";
        return this;
    }

    public KubernetesDeploymentTemplateData SetSecretsFromSecretState(KeyValuePair<string, Resource> resource, ISecretProvider secretProvider, bool? disableSecrets = false)
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
    public bool HasAnySecrets => Secrets?.Any() == true;
    public bool WithNamespace => !string.IsNullOrWhiteSpace(Namespace);

    public KubernetesDeploymentTemplateData Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Name must be set.");
        }

        return this;
    }
}
