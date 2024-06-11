namespace Aspirate.Tests.ServiceTests;

public class ProjectPropertyServiceTest
{
    [Fact]
    public async Task GetProjectPropertiesAsync_ShouldCallExpectedMethods_WhenCalled()
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ProjectPropertyService(fileSystem, shellExecutionService, new TestConsole());

        var projectPath = "testPath";
        var propertyNames = new[] { "Property1", "Property2" };

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, "test", string.Empty, 0)));

        // Act
        var result = await service.GetProjectPropertiesAsync(projectPath, propertyNames);

        // Assert
        await shellExecutionService.Received(1).ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null));
        Assert.NotNull(result);
    }
}

