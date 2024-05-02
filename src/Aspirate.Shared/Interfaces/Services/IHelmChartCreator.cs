namespace Aspirate.Shared.Interfaces.Services;

public interface IHelmChartCreator
{
    Task CreateHelmChart(string kustomizePath, string chartPath, string chartName);
}
