namespace Aspirate.DockerCompose.Builders;

public class DeployBuilder : BaseBuilder<DeployBuilder, Deploy>
{
    internal DeployBuilder()
    {
    }

    public DeployBuilder WithLabels(IDictionary<string, string> labels)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        return AddToDictionary(WorkingObject.Labels, labels);
    }

    public DeployBuilder WithLabels(Action<IDictionary<string, string>> environmentExpression)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        environmentExpression(WorkingObject.Labels);

        return this;
    }

    public DeployBuilder WithMode(ReplicationMode mode)
    {
        WorkingObject.Mode = mode;
        return this;
    }

    public DeployBuilder WithReplicas(int replicas)
    {
        WorkingObject.Replicas = replicas;
        return this;
    }

    public DeployBuilder WithUpdateConfig(Action<MapBuilder> updateConfig)
    {
        var mb = new MapBuilder();
        updateConfig(mb);
        WorkingObject.UpdateConfig = mb.Build();
        return this;
    }

    public DeployBuilder WithRestartPolicy(Action<MapBuilder> restartPolicy)
    {
        var mb = new MapBuilder();
        restartPolicy(mb);
        WorkingObject.RestartPolicy = mb.Build();
        return this;
    }

    public DeployBuilder WithPlacement(Action<MapBuilder> placement)
    {
        var mb = new MapBuilder();
        placement(mb);
        WorkingObject.Placement = mb.Build();
        return this;
    }

    public DeployBuilder WithResources(Action<MapBuilder> resources)
    {
        var mb = new MapBuilder();
        resources(mb);
        WorkingObject.Resources = mb.Build();
        return this;
    }
}
