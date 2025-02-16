namespace Aspirate.Processors.Transformation.Bindings;

public sealed class BindingProcessor : IBindingProcessor
{
    private const int DefaultServicePort = 10000;
    private static int _servicePort = DefaultServicePort;

    public static IBindingProcessor CreateDefaultExpressionProcessor() =>
        new BindingProcessor();

    public void ResetServicePort() =>
        _servicePort = DefaultServicePort;

    public string? ParseBinding(IReadOnlyList<string> pathParts, JsonNode? rootNode)
    {
        var resourceName = pathParts[0];
        var bindingName = pathParts[2];
        var bindingProperty = pathParts[3];

        return ParseBinding(resourceName, bindingName, bindingProperty, rootNode);
    }

    private static string? ParseBinding(string resourceName, string bindingName, string bindingProperty, JsonNode? rootNode)
    {
        var bindingEntry = rootNode[resourceName][Literals.Bindings][bindingName].Deserialize<Binding>();

        return bindingProperty switch
        {
            Literals.Host => resourceName,  // return the name of the resource for 'host'
            Literals.Port => bindingEntry.Port.GetValueOrDefault() != 0 ? bindingEntry.Port.ToString() : bindingEntry.TargetPort.ToString(),
            Literals.TargetPort => bindingEntry.TargetPort.ToString(),
            Literals.Url => HandleUrlBinding(resourceName, bindingName, bindingEntry),
            Literals.Scheme => bindingEntry.Scheme,
            _ => throw new InvalidOperationException($"Unknown property {bindingProperty}.")
        };
    }

    private static string HandleUrlBinding(string resourceName, string bindingName, Binding binding) =>
        bindingName switch
        {
            Literals.Http => $"{Literals.Http}://{resourceName}:{binding.TargetPort}",
            Literals.Https => string.Empty, // For now - disable https, only http is supported until we have a way to generate dev certs and inject into container for startup.
            _ => HandleCustomServicePortBinding(resourceName, binding),
        };

    private static string HandleCustomServicePortBinding(string resourceName, Binding binding)
    {
        if (binding.TargetPort == 0)
        {
            binding.TargetPort = _servicePort;
            _servicePort++;
        }

        var prefix = HandleServiceBindingPrefix(binding);

        return $"{prefix}{resourceName}:{binding.TargetPort}";
    }

    private static string HandleServiceBindingPrefix(Binding binding) =>
        binding.Protocol switch
        {
            Literals.Http => $"{Literals.Http}://",
            Literals.Https => $"{Literals.Https}://",
            _ => string.Empty,
        };
}
