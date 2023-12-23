namespace Aspirate.DockerCompose.Builders;

public abstract class BaseBuilder<TBuilderType, TObjectType>
    where TObjectType : class, new()
    where TBuilderType : BaseBuilder<TBuilderType, TObjectType>
{
    protected TObjectType WorkingObject { get; set; } = new();

    protected TBuilderType AddToDictionary<T>(IDictionary<string, T> original, IDictionary<string, T> source)
    {
        foreach (var item in source)
        {
            original[item.Key] = item.Value;
        }

        return (TBuilderType) this;
    }

    public virtual TObjectType Build() => WorkingObject;
}
