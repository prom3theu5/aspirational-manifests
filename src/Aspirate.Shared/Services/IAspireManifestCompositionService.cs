namespace Aspirate.Shared.Services;

public interface IAspireManifestCompositionService
{
    Task<(bool Success, string FullPath)> BuildManifestForProject(string appHostProject);
}
