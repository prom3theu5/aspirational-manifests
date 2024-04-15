using System.Text.Json;

namespace Aspirate.Services.Extensions;
public static class JsonExtensions
{
    public static JsonDocument TryParseJson(this string json)
    {
        try
        {
            return JsonDocument.Parse(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
