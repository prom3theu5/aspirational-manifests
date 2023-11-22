namespace Aspirate.Contracts.Interfaces;

public interface IContainerCompositionService
{
    Task<bool> BuildAndPushContainerForProject(Project project);
}
