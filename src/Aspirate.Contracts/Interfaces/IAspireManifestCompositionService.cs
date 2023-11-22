namespace Aspirate.Contracts.Interfaces;

public interface IAspireManifestCompositionService
{
    Task<(bool Success, string FullPath)> BuildManifestForProject(string appHostProject);
}
