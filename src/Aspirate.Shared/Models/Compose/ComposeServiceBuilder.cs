using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Builders.Services;
using DockerComposeBuilder.Enums;

namespace Aspirate.Shared.Models.Compose;

public class ComposeServiceBuilder : BaseBuilder<ComposeServiceBuilder, ComposeService>
{
    protected BuildBuilder? BuildBuilder;

    public ComposeServiceBuilder()
    {
    }

    public ComposeServiceBuilder WithName(string name)
    {
        WorkingObject.Name = name;

        return this;
    }

    public ComposeServiceBuilder WithContainerName(string containerName)
    {
        WorkingObject.ContainerName = containerName;
        return this;
    }

    public ComposeServiceBuilder WithDependencies(params string[] services)
    {
        WorkingObject.DependsOn ??= new List<string>();

        var dependsOnList = WorkingObject.DependsOn;

        dependsOnList?.AddRange(services);
        return this;
    }

    public ComposeServiceBuilder WithDependencies(params Service[] services)
    {
        return WithDependencies(services.Select(t => t.Name).ToArray());
    }

    public ComposeServiceBuilder WithHealthCheck(Action<HealthCheck> healthCheck)
    {
        WorkingObject.HealthCheck ??= new HealthCheck();

        healthCheck(WorkingObject.HealthCheck);

        return this;
    }

    public ComposeServiceBuilder WithEnvironment(IDictionary<string, string?> environmentMappings)
    {
        WorkingObject.Environment ??= new Dictionary<string, string?>();

        return AddToDictionary(WorkingObject.Environment, environmentMappings);
    }

    public ComposeServiceBuilder WithEnvironment(Action<IDictionary<string, string?>> environmentExpression)
    {
        WorkingObject.Environment ??= new Dictionary<string, string?>();

        environmentExpression(WorkingObject.Environment);

        return this;
    }

    public ComposeServiceBuilder WithExposed(params string[] exposed)
    {
        if (WorkingObject.Expose == null)
        {
            WorkingObject.Expose = new List<string>();
        }

        WorkingObject.Expose.AddRange(exposed);
        return this;
    }

    public ComposeServiceBuilder WithExposed(params object[] exposed)
    {
        WorkingObject.Expose ??= new List<string>();

        WorkingObject.Expose.AddRange(exposed.Select(t => t.ToString())!);
        return this;
    }

    public ComposeServiceBuilder WithHostname(string hostname)
    {
        WorkingObject.Hostname = hostname;
        return this;
    }

    public ComposeServiceBuilder WithNetworkMode(string networkMode)
    {
        WorkingObject.NetworkMode = networkMode;
        return this;
    }

    public ComposeServiceBuilder WithLabels(IDictionary<string, string> labels)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        return AddToDictionary(WorkingObject.Labels, labels);
    }

    public ComposeServiceBuilder WithLabels(Action<IDictionary<string, string>> environmentExpression)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        environmentExpression(WorkingObject.Labels);

        return this;
    }

    public ComposeServiceBuilder WithBuild(Action<BuildBuilder> build)
    {
        BuildBuilder ??= new BuildBuilder();

        build(BuildBuilder);

        return this;
    }

    public ComposeServiceBuilder WithImage(string image)
    {
        WorkingObject.Image = image;
        return this;
    }

    public ComposeServiceBuilder WithPrivileged(bool? privileged = true)
    {
        WorkingObject.Privileged = privileged;
        return this;
    }

    public ComposeServiceBuilder WithNetworks(params string[] networks)
    {
        if (WorkingObject.Networks == null)
        {
            WorkingObject.Networks = new List<string>();
        }

        WorkingObject.Networks.AddRange(networks);
        return this;
    }

    public ComposeServiceBuilder WithNetworks(params Network[] networks)
    {
        if (WorkingObject.Networks == null)
        {
            WorkingObject.Networks = new List<string>();
        }

        WorkingObject.Networks.AddRange(networks.Select(t => t.Name));
        return this;
    }

    public ComposeServiceBuilder WithPortMappings(params Port[] mappings)
    {
        if (WorkingObject.Ports == null)
        {
            WorkingObject.Ports = new List<Port>();
        }

        WorkingObject.Ports.AddRange(mappings);
        return this;
    }

    public ComposeServiceBuilder WithExtraHosts(params string[] extraHosts)
    {
        if (WorkingObject.ExtraHosts == null)
        {
            WorkingObject.ExtraHosts = new List<string>();
        }

        WorkingObject.ExtraHosts.AddRange(extraHosts);
        return this;
    }

    public ComposeServiceBuilder WithCommands(params string[] commands)
    {
        if (WorkingObject.Commands == null)
        {
            WorkingObject.Commands = new List<string>();
        }

        WorkingObject.Commands.AddRange(commands);
        return this;
    }

    public ComposeServiceBuilder WithRestartPolicy(ERestartMode mode)
    {
        WorkingObject.Restart = mode;
        return this;
    }

    public ComposeServiceBuilder WithSecrets(params string[] secrets)
    {
        if (WorkingObject.Secrets == null)
        {
            WorkingObject.Secrets = new List<string>();
        }

        WorkingObject.Secrets.AddRange(secrets);
        return this;
    }

    public ComposeServiceBuilder WithSecrets(params Secret[] secrets)
    {
        return WithSecrets(secrets.Select(t => t.Name).ToArray());
    }

    public ComposeServiceBuilder WithVolumes(params string[] volumes)
    {
        if (WorkingObject.Volumes == null)
        {
            WorkingObject.Volumes = new List<string>();
        }

        WorkingObject.Volumes.AddRange(volumes);
        return this;
    }

    public override ComposeService Build()
    {
        if (BuildBuilder != null)
        {
            WorkingObject.Build = BuildBuilder.Build();
        }

        return base.Build();
    }
}
