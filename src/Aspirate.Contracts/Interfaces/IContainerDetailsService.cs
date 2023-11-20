using Aspirate.Contracts.Models.Containers;

namespace Aspirate.Contracts.Interfaces;
public interface IContainerDetailsService
{
    Task<ContainerDetails> GetContainerDetails(string resourceName, Project project);
    string GetDefaultImageName(Project project);
    string GetFullImage(ContainerDetails containerDetails, string defaultImageName);
}
