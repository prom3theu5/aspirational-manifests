namespace Aspirate.Shared.Interfaces.Services;

/// <summary>
/// Represents a service for building manifests for Aspire projects.
/// </summary>
public interface IAspireManifestCompositionService
{
    /// <summary>
    /// Builds a manifest for the specified application host project.
    /// </summary>
    /// <param name="appHostProject">The path to the application host project.</param>
    /// <param name="launchProfile">The aspire launch profile to use.</param>
    /// <returns>A tuple indicating the success status and the full path of the built manifest.</returns>
    Task<(bool Success, string FullPath)> BuildManifestForProject(string appHostProject, string? launchProfile);
}
