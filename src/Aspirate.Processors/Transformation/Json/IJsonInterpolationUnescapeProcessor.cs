namespace Aspirate.Processors.Transformation.Json;

public interface IJsonInterpolationUnescapeProcessor
{
    void UnescapeJsonExpression(JsonNode node);
}
