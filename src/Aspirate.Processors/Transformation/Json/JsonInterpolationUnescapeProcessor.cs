using Json.More;

namespace Aspirate.Processors.Transformation.Json;

public class JsonInterpolationUnescapeProcessor : IJsonInterpolationUnescapeProcessor
{
    public static IJsonInterpolationUnescapeProcessor CreateDefaultInterpolationUnescapeProcessor() =>
        new JsonInterpolationUnescapeProcessor();

    public void UnescapeJsonExpression(JsonNode? jsonNode)
    {
        if (jsonNode is null)
        {
            return;
        }

        switch (jsonNode)
        {
            case JsonObject jsonObject:
                UnescapeJsonExpression(jsonObject);
                break;

            case JsonArray jsonArray:
                UnescapeJsonExpression(jsonArray);
                break;

            case JsonValue jsonValue:
                UnescapeJsonExpression(jsonValue);
                break;

            default:
                throw new InvalidOperationException($"Unexpected node type: {jsonNode.GetType().Name}");
        }
    }

    private void UnescapeJsonExpression(JsonObject jsonObject)
    {
        foreach (var kvp in jsonObject.ToArray())
        {
            UnescapeJsonExpression(kvp.Value);
        }
    }

    private void UnescapeJsonExpression(JsonArray jsonArray)
    {
        foreach (var element in jsonArray.ToArray())
        {
            UnescapeJsonExpression(element);
        }
    }

    private void UnescapeJsonExpression(JsonValue jsonValue)
    {
        if (jsonValue.GetValueKind() == JsonValueKind.String)
        {
            var strValue = jsonValue.GetString();

            if (strValue != null)
            {
                jsonValue.ReplaceWith(JsonInterpolation.Unescape(strValue));
            }
        }
    }
}
