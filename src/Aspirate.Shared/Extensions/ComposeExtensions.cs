using DockerComposeBuilder.Converters;

namespace Aspirate.Shared.Extensions;

public static class ComposeExtensions
{
    public static string Serialize(this Compose serializable, string lineEndings = "\n")
    {
        var serializer = new SerializerBuilder()
            .WithTypeConverter(new YamlValueCollectionConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithEventEmitter(nextEmitter => new FlowStyleStringSequences(nextEmitter))
            .WithEventEmitter(nextEmitter => new FlowStringEnumConverter(nextEmitter))
            .WithEventEmitter(nextEmitter => new ForceQuotedStringValuesEventEmitter(nextEmitter))
            .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
            .WithNewLine(lineEndings)
            .Build();

        return serializer.Serialize(serializable);
    }
}
