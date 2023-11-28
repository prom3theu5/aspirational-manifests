namespace Aspirate.Shared.Services;

public interface IContainerCompositionService
{
    Task<bool> BuildAndPushContainerForProject(Project project, MsBuildContainerProperties containerDetails, bool nonInteractive);
    Task<bool> BuildAndPushContainerForDockerfile(Dockerfile dockerfile, string builder, string imageName, string registry, bool nonInteractive);
}
