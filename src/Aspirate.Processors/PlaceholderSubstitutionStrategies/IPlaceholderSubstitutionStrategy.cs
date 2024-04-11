namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public interface IPlaceholderSubstitutionStrategy
{
    bool CanSubstitute(KeyValuePair<string, string> placeholder, bool ignorePlaceholder = false);
    void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource);
    void Reset();
}
