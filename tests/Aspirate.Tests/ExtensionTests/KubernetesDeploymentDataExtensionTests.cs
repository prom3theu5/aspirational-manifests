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
            .SetEnv(new Dictionary<string, string> { { "key", "value" } });

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
            .SetSecrets(new Dictionary<string, string> { { "key", "value" } });

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
    public void ToKubernetesStatefulSet_ShouldReturnCorrectStatefulSet()
    {
        // Arrange
        var data = new KubernetesDeploymentData()
            .SetName("test")
            .SetContainerImage("test-image");

        // Act
        var result = data.ToKubernetesStatefulSet();

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
            .SetPorts(new List<Ports> { new Ports { Name = "test-port", InternalPort = 8080, ExternalPort = 8080 } });

        // Act
        var result = data.ToKubernetesObjects();

        // Assert
        result.Should().ContainItemsAssignableTo<V1Deployment>();
        result.Should().ContainItemsAssignableTo<V1Service>();
    }
}
