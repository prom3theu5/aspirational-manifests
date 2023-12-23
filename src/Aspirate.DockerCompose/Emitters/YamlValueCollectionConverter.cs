namespace Aspirate.DockerCompose.Emitters;

public class YamlValueCollectionConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => typeof(IValueCollection).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value is not IValueCollection valueCollection)
        {
            return;
        }

        emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Block));

        foreach (var item in valueCollection)
        {
            switch (item)
            {
                case IKeyValue keyValue:
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, $"{keyValue.Key}={keyValue.Value}", ScalarStyle.DoubleQuoted, true, false));
                    break;
                case IKey key:
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, key.Key, ScalarStyle.DoubleQuoted, true, false));
                    break;
            }
        }

        emitter.Emit(new SequenceEnd());
    }
}
