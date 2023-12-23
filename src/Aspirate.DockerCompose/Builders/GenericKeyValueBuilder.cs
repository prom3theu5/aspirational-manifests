namespace Aspirate.DockerCompose.Builders;

public abstract class GenericKeyValueBuilder<TBuilder, TBaseItem, TItemValueLess, TItem>
    where TBuilder : GenericKeyValueBuilder<TBuilder, TBaseItem, TItemValueLess, TItem>
    where TBaseItem : class
    where TItemValueLess : class, TBaseItem, IKey, new()
    where TItem : class, TBaseItem, IKeyValue, new()
{
    private readonly List<TBaseItem> _internalCollection = [];

    public TBuilder Add(params TBaseItem[] buildArgument)
    {
        _internalCollection.AddRange(buildArgument);

        return (TBuilder) this;
    }

    public TBuilder AddWithoutValue(params string[] keys)
    {
        _internalCollection.AddRange(keys.Select(x => new TItemValueLess
        {
            Key = x,
        }));

        return (TBuilder) this;
    }

    public TBuilder Add(params KeyValuePair<string, string>[] items)
    {
        _internalCollection.AddRange(items.Select(x => new TItem
        {
            Key = x.Key,
            Value = x.Value,
        }));

        return (TBuilder) this;
    }

    public IValueCollection<TBaseItem> Build() => new ValueCollection<TBaseItem>(_internalCollection);
}
