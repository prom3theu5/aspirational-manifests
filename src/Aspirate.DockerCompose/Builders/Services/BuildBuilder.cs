namespace Aspirate.DockerCompose.Builders.Services;

public class BuildBuilder : BaseBuilder<BuildBuilder, ServiceBuild>
{
    private BuildArgumentBuilder? _buildArgumentBuilder;

    public BuildBuilder WithContext(string context)
    {
        WorkingObject.Context = context;

        return this;
    }

    public BuildBuilder WithDockerfile(string dockerfile)
    {
        WorkingObject.Dockerfile = dockerfile;

        return this;
    }

    public BuildBuilder WithArguments(Action<BuildArgumentBuilder> argumentsBuilder)
    {
        _buildArgumentBuilder ??= new();

        argumentsBuilder(_buildArgumentBuilder);

        return this;
    }

    public override ServiceBuild Build()
    {
        if (_buildArgumentBuilder != null)
        {
            WorkingObject.Arguments = _buildArgumentBuilder.Build();
        }

        return base.Build();
    }
}
