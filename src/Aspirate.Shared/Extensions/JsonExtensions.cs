using System.Text.Json.Nodes;

namespace Aspirate.Shared.Extensions;
public static class JsonExtensions
{
    public static JsonDocument? TryParseAsJsonDocument<T>(this T? instance)
    {
        try
        {
            if (instance is null)
            {
                return null;
            }

            if (instance is string jsonString)
            {
                return JsonDocument.Parse(jsonString);
            }

            return JsonDocument.Parse(JsonSerializer.Serialize(instance));
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static JsonNode? TryParseAsJsonNode<T>(this T? instance)
    {
        try
        {
            if (instance is null)
            {
                return null;
            }

            if (instance is string jsonString)
            {
                return JsonNode.Parse(jsonString);
            }

            return JsonNode.Parse(JsonSerializer.Serialize(instance));
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static T ToObject<T>(this JsonNode jsonNode) =>
        JsonSerializer.Deserialize<T>(jsonNode.ToString())!;

    public static T ToObject<T>(this JsonDocument jsonDocument) =>
        JsonSerializer.Deserialize<T>(jsonDocument.RootElement.GetRawText())!;
}
