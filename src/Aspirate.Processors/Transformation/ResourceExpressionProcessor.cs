namespace Aspirate.Processors.Transformation;

public sealed class ResourceExpressionProcessor(IJsonExpressionProcessor jsonExpressionProcessor) : IResourceExpressionProcessor
{
    public static IResourceExpressionProcessor CreateDefaultExpressionProcessor() =>
        new ResourceExpressionProcessor(JsonExpressionProcessor.CreateDefaultExpressionProcessor());

    public void ProcessEvaluations(Dictionary<string, Resource> resources)
    {
        resources.EnsureBindingsHavePorts();
        var jsonDocument = resources.TryParseAsJsonNode();
        var rootNode = jsonDocument.Root;

        jsonExpressionProcessor.ResolveJsonExpressions(rootNode, rootNode);

        HandleSubstitutions(resources, rootNode);
    }

    private static void HandleSubstitutions(Dictionary<string, Resource> resources, JsonNode rootNode)
    {
        foreach (var (key, value) in resources)
        {
            switch (value)
            {
                case IResourceWithConnectionString resourceWithConnectionString when !string.IsNullOrEmpty(resourceWithConnectionString.ConnectionString):
                    resourceWithConnectionString.ConnectionString = rootNode[key]![Literals.ConnectionString]!.ToString();
                    break;
                case ValueResource valueResource:
                    {
                        foreach (var resourceValue in valueResource.Values.ToList())
                        {
                            valueResource.Values[resourceValue.Key] = rootNode[key]![resourceValue.Key]!.ToString();
                        }

                        break;
                    }
            }

            if (value is IResourceWithEnvironmentalVariables resourceWithEnvVars && resourceWithEnvVars.Env is not null)
            {
                foreach (var envVar in resourceWithEnvVars.Env)
                {
                    resourceWithEnvVars.Env[envVar.Key] = rootNode[key]![Literals.Env]![envVar.Key]!.ToString();
                }
            }
        }
    }
}
