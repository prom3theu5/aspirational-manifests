using Json.More;

namespace Aspirate.Processors.Transformation.Json;

public sealed partial class JsonExpressionProcessor(IBindingProcessor bindingProcessor) : IJsonExpressionProcessor
{
    private readonly ICollection<string> _unresolvedExpressionPointers = [];

    public void ResolveJsonExpressions(JsonNode? jsonNode, JsonNode rootNode)
    {
        _unresolvedExpressionPointers.Clear();
        ResolveJsonExpressionsRecursive(jsonNode, rootNode);
        do
        {
            var pointers = _unresolvedExpressionPointers.ToList();
            _unresolvedExpressionPointers.Clear();
            var list = pointers
                .Select(pointer => pointer
                    .Remove(0, 1)
                    .Split("/")
                    .Aggregate(rootNode, (current, path) => int.TryParse(path, out var index) ? current[index] : current[path]));
            foreach (var node in list)
            {
                HandleJsonValue(rootNode, node);
            }
        } while (_unresolvedExpressionPointers.Count > 0);
    }
    public static IJsonExpressionProcessor CreateDefaultExpressionProcessor() =>
        new JsonExpressionProcessor(BindingProcessor.CreateDefaultExpressionProcessor());

    private void ResolveJsonExpressionsRecursive(JsonNode? jsonNode, JsonNode rootNode)
    {
        if (jsonNode is null)
        {
            return;
        }

        switch (jsonNode)
        {
            case JsonObject jsonObject:
                HandleJsonObject(rootNode, jsonObject);
                break;
            case JsonArray jsonArray:
                HandleJsonArray(rootNode, jsonArray);
                break;
            case JsonValue jsonValue:
                HandleJsonValue(rootNode, jsonValue);
                break;
            default:
                throw new InvalidOperationException($"Unexpected node type: {jsonNode.GetType().Name}");
        }
    }


    private void HandleJsonObject(JsonNode rootNode, JsonObject jsonObject)
    {
        var keys = new List<string>(((IDictionary<string, JsonNode?>)jsonObject).Keys);

        foreach (var item in keys.Select(key => jsonObject[key]))
        {
            if (item is JsonValue jsonValue)
            {
                ResolveJsonExpressionsRecursive(jsonValue, rootNode);
                continue;
            }

            ResolveJsonExpressionsRecursive(item, rootNode);
        }
    }

    private void HandleJsonArray(JsonNode rootNode, JsonArray jsonArray)
    {
        foreach (var item in jsonArray.Where(item => item is JsonArray))
        {
            if (item is JsonArray)
            {
                ResolveJsonExpressionsRecursive(item, rootNode);
            }
        }
    }

    private void HandleJsonValue(JsonNode rootNode, JsonNode jsonValue) => ReplaceWithResolvedExpression(rootNode, jsonValue);

    private static readonly Regex PlaceholderPatternRegex = new(@"{([\w.-]+)}", RegexOptions.Compiled);
    private void ReplaceWithResolvedExpression(JsonNode rootNode, JsonNode jsonValue)
    {
        var input = jsonValue.ToString();
        var inputBefore = input;

        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        if (!input.Contains('{', StringComparison.OrdinalIgnoreCase) || !input.Contains('}', StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var matches = PlaceholderPatternRegex.Matches(input);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var jsonPath = match.Groups[1].Value;
            var pathParts = jsonPath.Split('.');
            if (pathParts.Length == 1)
            {
                input = input.Replace($"{{{jsonPath}}}", rootNode[pathParts[0]].ToString(), StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (pathParts is [_, Literals.Bindings, ..])
            {
                input = bindingProcessor.HandleBindingReplacement(rootNode, pathParts, input, jsonPath);
                continue;
            }

            var selectionPath = pathParts.AsJsonPath();
            var path = JsonPath.Parse(selectionPath);
            var result = path.Evaluate(rootNode);

            if (result.Matches.Count == 0)
            {
                continue;
            }

            var value = result.Matches.FirstOrDefault()?.Value;

            if (value is null)
            {
                continue;
            }

            input = input.Replace($"{{{jsonPath}}}", value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        var pointer = jsonValue.GetPointerFromRoot();
        jsonValue.ReplaceWith(input);

        if (!string.Equals(inputBefore, input, StringComparison.OrdinalIgnoreCase) &&
            input.Contains('{', StringComparison.OrdinalIgnoreCase) && input.Contains('}', StringComparison.OrdinalIgnoreCase))
        {
            _unresolvedExpressionPointers.Add(pointer);
        }
    }
}
