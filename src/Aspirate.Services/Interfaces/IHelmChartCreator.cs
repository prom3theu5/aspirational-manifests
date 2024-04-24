namespace Aspirate.Services.Interfaces;

public interface IHelmChartCreator
{
    Task CreateHelmChart(string kustomizePath, string chartPath, string chartName);
}
