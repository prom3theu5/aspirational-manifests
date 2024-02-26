namespace Aspirate.Tests.ServiceTests;

public class KubeCtlServiceTests
{
    private const string InputPath = "/some-path";
    private const string TestNamespace = "test-namespace";

    private const string TestNamespaceYamlFileContents =
        $"""
        apiVersion: v1
        kind: Namespace
        metadata:
          name: {TestNamespace}
        """;

    [Fact]
    public async Task PerformRollingRestart_WhenCustomNamespaceIsProvided_ShouldReturnTrue()
    {
        // Arrange
        var testFileSystem = SetupTestFilesystem();
        var shellExecutionService = Substitute.For<IShellExecutionService>();
        var console = Substitute.For<IAnsiConsole>();
        var kubeCtlService = new KubeCtlService(testFileSystem, console, shellExecutionService);

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, string.Empty, string.Empty, 0)));

        // Act
        var result = await kubeCtlService.PerformRollingRestart(TestNamespace, InputPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PerformRollingRestart_WhenNoCustomNamespaceIsProvided_ShouldReturnTrue()
    {
        // Arrange
        var testFileSystem = new MockFileSystem();
        var shellExecutionService = Substitute.For<IShellExecutionService>();
        var console = Substitute.For<IAnsiConsole>();
        var kubeCtlService = new KubeCtlService(testFileSystem, console, shellExecutionService);

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, string.Empty, string.Empty, 0)));

        // Act
        var result = await kubeCtlService.PerformRollingRestart(TestNamespace, InputPath);

        // Assert
        Assert.True(result);
    }

    private static MockFileSystem SetupTestFilesystem()
    {
        var testFileSystem = new MockFileSystem();
        testFileSystem.AddDirectory(InputPath);
        var inputPath = testFileSystem.NormalizePath(InputPath);
        var configurationFile = testFileSystem.Path.Combine(inputPath, $"{TemplateLiterals.NamespaceType}.yaml");
        testFileSystem.AddFile(configurationFile, TestNamespaceYamlFileContents);

        return testFileSystem;
    }
}
