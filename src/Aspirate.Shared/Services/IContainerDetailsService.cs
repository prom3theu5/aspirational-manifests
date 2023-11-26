namespace Aspirate.Shared.Services;
public interface IContainerDetailsService
{
    Task<MsBuildContainerProperties> GetContainerDetails(string resourceName, Project project, string? containerRegistry, string? containerImageTag);
}
