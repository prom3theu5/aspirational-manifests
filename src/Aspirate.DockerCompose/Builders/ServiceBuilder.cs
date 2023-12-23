namespace Aspirate.DockerCompose.Builders;

public class ServiceBuilder : BaseBuilder<ServiceBuilder, Service>
{
    private BuildBuilder? _buildBuilder;

    internal ServiceBuilder()
    {
    }

    public ServiceBuilder WithName(string name)
    {
        WorkingObject.Name = name;

        return this;
    }

    public ServiceBuilder WithContainerName(string containerName)
    {
        WorkingObject.ContainerName = containerName;
        return this;
    }

    private ServiceBuilder WithDependencies(params string[] services)
    {
        WorkingObject.DependsOn ??= [];

        var dependsOnList = WorkingObject.DependsOn;

        dependsOnList?.AddRange(services);
        return this;
    }

    public ServiceBuilder WithDependencies(params Service[] services) => WithDependencies(services.Select(t => t.Name).ToArray());

    public ServiceBuilder WithHealthCheck(Action<HealthCheck> healthCheck)
    {
        WorkingObject.HealthCheck ??= new();

        healthCheck(WorkingObject.HealthCheck);

        return this;
    }

    public ServiceBuilder WithEnvironment(IDictionary<string, string?> environmentMappings)
    {
        WorkingObject.Environment ??= new Dictionary<string, string?>();

        return AddToDictionary(WorkingObject.Environment, environmentMappings);
    }

    public ServiceBuilder WithEnvironment(Action<IDictionary<string, string?>> environmentExpression)
    {
        WorkingObject.Environment ??= new Dictionary<string, string?>();

        environmentExpression(WorkingObject.Environment);

        return this;
    }

    public ServiceBuilder WithExposed(params string[] exposed)
    {
        WorkingObject.Expose ??= [];

        WorkingObject.Expose.AddRange(exposed);
        return this;
    }

    public ServiceBuilder WithExposed(params object[] exposed)
    {
        WorkingObject.Expose ??= [];

        WorkingObject.Expose.AddRange(exposed.Select(t => t.ToString() ?? string.Empty));
        return this;
    }

    public ServiceBuilder WithHostname(string hostname)
    {
        WorkingObject.Hostname = hostname;
        return this;
    }

    public ServiceBuilder WithLabels(IDictionary<string, string> labels)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        return AddToDictionary(WorkingObject.Labels, labels);
    }

    public ServiceBuilder WithLabels(Action<IDictionary<string, string>> environmentExpression)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        environmentExpression(WorkingObject.Labels);

        return this;
    }

    public ServiceBuilder WithBuild(Action<BuildBuilder> build)
    {
        _buildBuilder ??= new();

        build(_buildBuilder);

        return this;
    }

    public ServiceBuilder WithImage(string image)
    {
        WorkingObject.Image = image;
        return this;
    }

    public ServiceBuilder WithPrivileged(bool? privileged = true)
    {
        WorkingObject.Privileged = privileged;
        return this;
    }

    public ServiceBuilder WithNetworks(params string[] networks)
    {
        WorkingObject.Networks ??= [];

        WorkingObject.Networks.AddRange(networks);
        return this;
    }

    public ServiceBuilder WithNetworks(params Network[] networks)
    {
        WorkingObject.Networks ??= [];

        WorkingObject.Networks.AddRange(networks.Select(t => t.Name));
        return this;
    }

    public ServiceBuilder WithPortMappings(params Port[] mappings)
    {
        WorkingObject.Ports ??= [];

        WorkingObject.Ports.AddRange(mappings);
        return this;
    }

    public ServiceBuilder WithExtraHosts(params string[] extraHosts)
    {
        WorkingObject.ExtraHosts ??= [];

        WorkingObject.ExtraHosts.AddRange(extraHosts);
        return this;
    }

    public ServiceBuilder WithCommands(params string[] commands)
    {
        WorkingObject.Commands ??= [];

        WorkingObject.Commands.AddRange(commands);
        return this;
    }

    public ServiceBuilder WithRestartPolicy(RestartMode mode)
    {
        WorkingObject.Restart = mode;
        return this;
    }

    private ServiceBuilder WithSecrets(params string[] secrets)
    {
        WorkingObject.Secrets ??= [];

        WorkingObject.Secrets.AddRange(secrets);
        return this;
    }

    public ServiceBuilder WithSecrets(params Secret[] secrets) => WithSecrets(secrets.Select(t => t.Name).ToArray());

    public SwarmServiceBuilder WithSwarm() =>
        new()
        {
            WorkingObject = WorkingObject
        };

    public ServiceBuilder WithVolumes(params string[] volumes)
    {
        WorkingObject.Volumes ??= [];

        WorkingObject.Volumes.AddRange(volumes);
        return this;
    }

    public override Service Build()
    {
        if (_buildBuilder != null)
        {
            WorkingObject.Build = _buildBuilder.Build();
        }

        return base.Build();
    }
}
