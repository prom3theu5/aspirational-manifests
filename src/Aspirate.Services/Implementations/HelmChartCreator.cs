namespace Aspirate.Services.Implementations;

public class HelmChartCreator(IFileSystem fileSystem, IKubernetesService kubernetesService, IAnsiConsole logger) : IHelmChartCreator
{
    public async Task CreateHelmChart(List<object> kubernetesObjects, string chartPath, string chartName, bool includeDashboard)
    {
        if (fileSystem.Directory.Exists(chartPath))
        {
            fileSystem.Directory.Delete(chartPath, true);
        }

        fileSystem.Directory.CreateDirectory(chartPath);
        fileSystem.Directory.CreateDirectory(fileSystem.Path.Combine(chartPath, "templates"));

        if (includeDashboard)
        {
            CreateDashboardObjects(kubernetesObjects);
        }

        await ProcessObjects(kubernetesObjects, chartPath);

        await CreateChartFile(chartPath, chartName);

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Generating helm chart at [blue]{chartPath}[/]");
    }

    private void CreateDashboardObjects(List<object> kubernetesObjects)
    {
        var dashboardObjects = kubernetesService.CreateDashboardKubernetesObjects();
        kubernetesObjects.AddRange(dashboardObjects);
    }

    private static async Task ProcessObjects(List<object> resources, string chartPath)
    {
        var valuesImages = new Dictionary<string, string>();

        foreach (var resource in resources)
        {
            switch (resource)
            {
                case V1ConfigMap configMap:
                    configMap.Metadata.NamespaceProperty = null;
                    await WriteResourceFile(KubernetesYaml.Serialize(configMap), chartPath, configMap.Metadata.Name, configMap.Kind);
                    continue;
                case V1Secret secret:
                    secret.Metadata.NamespaceProperty = null;
                    await WriteResourceFile(KubernetesYaml.Serialize(secret), chartPath, secret.Metadata.Name, secret.Kind);
                    continue;
                case V1Deployment deployment:
                    deployment.Metadata.NamespaceProperty = null;
                    await HandleDeployment(deployment, chartPath, valuesImages);
                    continue;
                case V1StatefulSet statefulSet:
                    statefulSet.Metadata.NamespaceProperty = null;
                    await HandleStatefulSet(statefulSet, chartPath, valuesImages);
                    continue;
                case V1Service service:
                    service.Metadata.NamespaceProperty = null;
                    await WriteResourceFile(KubernetesYaml.Serialize(service), chartPath, service.Metadata.Name, service.Kind);
                    continue;
            }
        }

        var values = new Dictionary<object, object>
        {
            ["images"] = valuesImages,
        };

        await CreateValuesFile(values, chartPath);
    }

    private static Task HandleStatefulSet(V1StatefulSet statefulSet, string chartPath, Dictionary<string, string> valuesImages)
    {
        var metadata = statefulSet.Metadata;
        var name = metadata.Name;
        var kind = statefulSet.Kind;

        PopulateValuesImages(statefulSet.Spec.Template.Spec.Containers, valuesImages, name);

        var updatedResource = KubernetesYaml.Serialize(statefulSet);

        return WriteResourceFile(updatedResource, chartPath, name, kind);
    }

    private static Task HandleDeployment(V1Deployment deployment, string chartPath, Dictionary<string, string> valuesImages)
    {
        var metadata = deployment.Metadata;
        var name = metadata.Name;
        var kind = deployment.Kind;

        PopulateValuesImages(deployment.Spec.Template.Spec.Containers, valuesImages, name);

        var updatedResource = KubernetesYaml.Serialize(deployment);

        return WriteResourceFile(updatedResource, chartPath, name, kind);
    }

    private static Task WriteResourceFile(string updatedResource, string chartPath, string name, string kind)
    {
        var filename = $"{chartPath}/templates/{name.ToLower()}-{kind.ToLower()}.yaml";

        return File.WriteAllTextAsync(filename, updatedResource);
    }

    private static void PopulateValuesImages(IEnumerable<V1Container>? containers, IDictionary<string, string> valuesImages, string? name)
    {
        var formattedName = name.Replace("-", "").ToLowerInvariant();

        foreach (var container in containers)
        {
            var image = container.Image;

            valuesImages[formattedName] = image;
            container.Image = $"{{{{ .Values.images.{formattedName} }}}}";
        }
    }

    private static async Task CreateChartFile(string chartPath, string chartName)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var chartFile = $"{chartPath}/Chart.yaml";

        var chartContent = new Dictionary<object, object>
        {
            { "apiVersion", "v2" },
            { "name", chartName },
            { "description", "A Helm chart to Deploy your Aspire Project to Kubernetes." },
            { "appVersion", "1.0.0" },
            { "version", "1.0.0" }
        };

        var yaml = serializer.Serialize(chartContent);

        await File.WriteAllTextAsync(chartFile, yaml);
    }

    private static async Task CreateValuesFile(Dictionary<object, object> values, string chartPath)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var valuesFile = $"{chartPath}/values.yaml";
        var valuesYaml = serializer.Serialize(values);
        await File.AppendAllTextAsync(valuesFile, valuesYaml);
    }
}
