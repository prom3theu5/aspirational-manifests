namespace Aspirate.DockerCompose.Builders;

public abstract class BuilderBase<TBuilderType, TObjectType>
    where TObjectType : ObjectBase, new()
    where TBuilderType : BuilderBase<TBuilderType, TObjectType>
{
    private TObjectType WorkingObject { get; set; } = new();

    public TBuilderType WithName(string name)
    {
        WorkingObject.Name = name;
        return (TBuilderType) this;
    }

    protected TBuilderType WithProperty(string property, object value)
    {
        WorkingObject[property] = value;
        return (TBuilderType) this;
    }

    public TBuilderType WithMap(string mapName, Action<MapBuilder> mapConfigureExpression)
    {
        var mb = new MapBuilder().WithName(mapName);
        mapConfigureExpression(mb);
        WorkingObject[mapName] = mb.Build();
        return (TBuilderType) this;
    }

    public TBuilderType WithMap(Map map)
    {
        WorkingObject[map.Name] = map;
        return (TBuilderType) this;
    }

    public TObjectType Build() => WorkingObject;
}
