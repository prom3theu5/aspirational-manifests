namespace Aspirate.Services.YamlImplementations.NodeDeserializers;

public class ScalarNodeDeserializer : INodeDeserializer
{
    public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
    {
        if (reader.Current is Scalar scalar)
        {
            value = scalar.Value;
            reader.MoveNext();
            return true;
        }

        value = null;
        return false;
    }
}
