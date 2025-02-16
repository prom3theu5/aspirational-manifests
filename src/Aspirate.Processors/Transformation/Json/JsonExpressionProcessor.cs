using System.Text;
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

        var tokens = JsonInterpolation.Tokenize(input);
        var transformedInput = new StringBuilder();

        foreach (var token in tokens)
        {
            if (token.IsText())
            {
                transformedInput.Append(token.Lexeme);
            }
            else
            {
                var jsonPath = token.Lexeme;
                var pathParts = jsonPath.Split('.');

                void AppendPlaceholderTokenAsText()
                {
                    transformedInput.Append('{');
                    transformedInput.Append(jsonPath);
                    transformedInput.Append('}');
                }

                if (pathParts.Length == 1)
                {
                    var resolvedNode = rootNode[pathParts[0]];

                    if (resolvedNode != null)
                    {
                        transformedInput.Append(resolvedNode.ToString());
                    }
                    else
                    {
                        AppendPlaceholderTokenAsText();
                    }

                    continue;
                }
                else if (pathParts is [_, Literals.Bindings, ..])
                {
                    transformedInput.Append(bindingProcessor.ParseBinding(pathParts, rootNode));
                    continue;
                }

                var selectionPath = pathParts.AsJsonPath();
                var path = JsonPath.Parse(selectionPath);
                var result = path.Evaluate(rootNode);

                if (result.Matches.Count == 0)
                {
                    AppendPlaceholderTokenAsText();
                    continue;
                }

                var value = result.Matches.FirstOrDefault()?.Value;

                if (value is null)
                {
                    AppendPlaceholderTokenAsText();
                    continue;
                }

                transformedInput.Append(value.ToString());
            }
        }

        input = transformedInput.ToString();

        var pointer = jsonValue.GetPointerFromRoot();

        // If input is different, retokenize and see if any placeholders are present.
        if (!string.Equals(inputBefore, input, StringComparison.OrdinalIgnoreCase) &&
            JsonInterpolation.Tokenize(input).Any(x => x.IsPlaceholder()))
        {
            _unresolvedExpressionPointers.Add(pointer);
        }

        jsonValue.ReplaceWith(input);
    }
}
