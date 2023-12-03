namespace Aspirate.Services.Interfaces;
public interface IContainerDetailsService
{
    Task<MsBuildContainerProperties> GetContainerDetails(string resourceName, Project project, string? containerRegistry, string? containerImageTag);
}
