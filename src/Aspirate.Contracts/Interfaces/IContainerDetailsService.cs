namespace Aspirate.Contracts.Interfaces;
public interface IContainerDetailsService
{
    Task<MsBuildContainerProperties> GetContainerDetails(string resourceName, Project project);
}
