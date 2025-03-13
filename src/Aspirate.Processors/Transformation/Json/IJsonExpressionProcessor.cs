namespace Aspirate.Processors.Transformation.Json;

public interface IJsonExpressionProcessor
{
    void ResolveJsonExpressions(JsonNode? jsonNode, JsonNode rootNode);
}
