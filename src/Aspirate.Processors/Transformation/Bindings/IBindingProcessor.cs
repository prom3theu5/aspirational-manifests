namespace Aspirate.Processors.Transformation.Bindings;

public interface IBindingProcessor
{
    void ResetServicePort();
    string HandleBindingReplacement(JsonNode rootNode, IReadOnlyList<string> pathParts, string input, string jsonPath);
}
