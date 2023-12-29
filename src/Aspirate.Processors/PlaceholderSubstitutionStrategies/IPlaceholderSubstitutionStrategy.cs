namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public interface IPlaceholderSubstitutionStrategy
{
    bool CanSubstitute(KeyValuePair<string, string> placeholder);
    void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource);
    void Reset();
}
