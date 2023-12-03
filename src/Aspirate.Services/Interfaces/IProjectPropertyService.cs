namespace Aspirate.Services.Interfaces;

public interface IProjectPropertyService
{
    Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames);
}
