namespace Aspirate.Services.Interfaces;

/// <summary>
/// Represents a service for retrieving project properties.
/// </summary>
public interface IProjectPropertyService
{
    /// <summary>
    /// Retrieves the specified properties of a project asynchronously.
    /// </summary>
    /// <param name="projectPath">The path to the project.</param>
    /// <param name="propertyNames">The names of the properties to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a string array
    /// that contains the values of the specified properties. If a property does not exist or
    /// cannot be retrieved, its value in the result array will be null.
    /// </returns>
    Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames);
}
