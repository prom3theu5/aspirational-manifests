namespace Aspirate.Shared.Services;

public interface IContainerCompositionService
{
    Task<bool> BuildAndPushContainerForProject(Project project, MsBuildContainerProperties containerDetails);
}
