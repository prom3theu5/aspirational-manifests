namespace Aspirate.Processors.Transformation.Bindings;

public interface IBindingProcessor
{
    void ResetServicePort();
    string? ParseBinding(IReadOnlyList<string> pathParts, JsonNode? rootNode);
}
