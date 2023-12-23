namespace Aspirate.DockerCompose.Builders;

public class SwarmServiceBuilder : ServiceBuilder
{
    public SwarmServiceBuilder WithDeploy(Action<DeployBuilder> builderExpression)
    {
        var deployBuilder = new DeployBuilder();
        builderExpression(deployBuilder);

        return WithDeploy(deployBuilder.Build());
    }

    private SwarmServiceBuilder WithDeploy(Deploy deploy)
    {
        WorkingObject.Deploy = deploy;
        return this;
    }
}
