namespace Aspirate.Processors.Transformation;

public interface IResourceExpressionProcessor
{
    void ProcessEvaluations(Dictionary<string, Resource> resources);
}
