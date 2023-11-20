namespace Aspirate.Contracts.Interfaces;

public interface IProjectPropertyService
{
    Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames);
}
