namespace Aspirate.Contracts.Interfaces;
public interface IContainerDetailsService
{
    Task<ContainerDetails> GetContainerDetails(string resourceName, Project project);
    string GetFullImage(ContainerDetails containerDetails, Project project);
}
