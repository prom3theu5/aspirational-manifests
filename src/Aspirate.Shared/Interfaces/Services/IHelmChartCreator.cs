namespace Aspirate.Shared.Interfaces.Services;

public interface IHelmChartCreator
{
    Task CreateHelmChart(List<object> kubernetesObjects, string chartPath, string chartName, bool includeDashboard);
}
