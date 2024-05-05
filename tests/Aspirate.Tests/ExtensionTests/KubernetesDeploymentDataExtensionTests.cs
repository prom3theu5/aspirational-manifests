namespace Aspirate.Tests.ExtensionTests;

public class KubernetesDeploymentDataExtensionTests
{
    [Fact]
    public void ToKubernetesLabels_ShouldReturnCorrectLabels()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test");

        // Act
        var result = data.ToKubernetesLabels();

        // Assert
        result.Should().ContainKey("app");
        result["app"].Should().Be("test");
    }

    [Fact]
    public void ToKubernetesObjectMetaData_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetNamespace("namespace");

        // Act
        var result = data.ToKubernetesObjectMetaData("suffix");

        // Assert
        result.Name.Should().Be("test-suffix");
        result.NamespaceProperty.Should().Be("namespace");
    }

    [Fact]
    public void ToKubernetesConfigMap_ShouldReturnCorrectConfigMap()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetEnv(new Dictionary<string, string?> { { "key", "value" } });

        // Act
        var result = data.ToKubernetesConfigMap();

        // Assert
        result.Data.Should().ContainKey("key");
        result.Data["key"].Should().Be("value");
    }

    [Fact]
    public void ToKubernetesSecret_ShouldReturnCorrectSecret()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetSecrets(new Dictionary<string, string?> { { "key", "value" } });

        // Act
        var result = data.ToKubernetesSecret();

        // Assert
        result.StringData.Should().ContainKey("key");
        result.StringData["key"].Should().Be("value");
    }

    [Fact]
    public void ToKubernetesContainer_ShouldReturnCorrectContainer()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetContainerImage("test-image");

        // Act
        var result = data.ToKubernetesContainer();

        // Assert
        result.Name.Should().Be("test");
        result.Image.Should().Be("test-image");
    }

    [Fact]
    public void ToKubernetesDeployment_ShouldReturnCorrectDeployment()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetContainerImage("test-image");

        // Act
        var result = data.ToKubernetesDeployment();

        // Assert
        result.Spec.Template.Spec.Containers[0].Name.Should().Be("test");
        result.Spec.Template.Spec.Containers[0].Image.Should().Be("test-image");
    }

    [Fact]
    public void ToKubernetesService_ShouldReturnCorrectService()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetPorts(new List<Ports> { new Ports { Name = "test-port", InternalPort = 8080, ExternalPort = 8080 } });

        // Act
        var result = data.ToKubernetesService();

        // Assert
        result.Spec.Ports[0].Name.Should().Be("test-port");
        result.Spec.Ports[0].Port.Should().Be(8080);
    }

    [Fact]
    public void ToKubernetesObjects_ShouldReturnCorrectObjects()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetContainerImage("test-image")
            .SetPorts(new List<Ports> { new Ports { Name = "test-port", InternalPort = 8080, ExternalPort = 8080 } })
            .SetEnv(new Dictionary<string, string?> { { "key", "envvalue" } })
            .SetSecrets(new Dictionary<string, string?> { { "key", "secretvalue" } });

        // Act
        var result = data.ToKubernetesObjects();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().ContainItemsAssignableTo<V1Deployment>();
        result.Should().ContainItemsAssignableTo<V1Service>();
        result.Should().ContainItemsAssignableTo<V1ConfigMap>();
        result.Should().ContainItemsAssignableTo<V1Secret>();

        var deployment = result.OfType<V1Deployment>().First();
        deployment.Spec.Template.Spec.Containers[0].Name.Should().Be("test");
        deployment.Spec.Template.Spec.Containers[0].Image.Should().Be("test-image");

        var service = result.OfType<V1Service>().First();
        service.Spec.Ports[0].Name.Should().Be("test-port");
        service.Spec.Ports[0].Port.Should().Be(8080);

        var configMap = result.OfType<V1ConfigMap>().First();
        configMap.Data.Should().ContainKey("key");
        configMap.Data["key"].Should().Be("envvalue");

        var secret = result.OfType<V1Secret>().First();
        secret.StringData.Should().ContainKey("key");
        secret.StringData["key"].Should().Be("secretvalue");
    }

    [Fact]
    public void ToKubernetesStatefulSet_ShouldReturnCorrectStatefulSet()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetContainerImage("test-image")
            .SetVolumes(new List<Volume> { new Volume { Name = "test-volume", Target = "/data" } });

        // Act
        var result = data.ToKubernetesStatefulSet();

        // Assert
        result.Spec.Template.Spec.Containers[0].Name.Should().Be("test");
        result.Spec.Template.Spec.Containers[0].Image.Should().Be("test-image");
        result.Spec.Template.Spec.Containers[0].VolumeMounts[0].Name.Should().Be("test-volume");
        result.Spec.Template.Spec.Containers[0].VolumeMounts[0].MountPath.Should().Be("/data");
        result.Spec.VolumeClaimTemplates[0].Metadata.Name.Should().Be("test-volume");
    }
}
