namespace Aspirate.Shared.Services;

public interface IProjectPropertyService
{
    Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames);
}
