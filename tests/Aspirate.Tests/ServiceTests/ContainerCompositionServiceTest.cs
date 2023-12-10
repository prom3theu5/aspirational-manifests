using NSubstitute.Core;

namespace Aspirate.Tests.ServiceTests;

public class ContainerCompositionServiceTest
{
    [Fact]
    public async Task BuildAndPushContainerForProject_ShouldCallExpectedMethods_WhenCalled()
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var console = Substitute.For<IAnsiConsole>();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var project = new Project
        {
            Path = "testPath"
        };

        var containerDetails = new MsBuildContainerProperties();

        projectPropertyService.GetProjectPropertiesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(
                JsonSerializer.Serialize(
                    new MsBuildProperties<MsBuildPublishingProperties>
                    {
                        Properties = new()
                        {
                            PublishSingleFile = "true",
                            PublishTrimmed = "true",
                        },
                    }));

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, "test", string.Empty, 0)));

        // Act
        var result = await service.BuildAndPushContainerForProject(project, containerDetails);

        // Assert
        await shellExecutionService.Received(1).ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null));
        result.Should().BeTrue();
    }


    [Fact]
    public async Task BuildAndPushContainerForDockerfile_ShouldCallExpectedMethods_WhenCalled()
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var console = Substitute.For<IAnsiConsole>();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var dockerfile = new Dockerfile { Path = "testPath", Context = "testContext" };
        var builder = "testBuilder";
        var imageName = "testImageName";
        var registry = "testRegistry";

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, "test", string.Empty, 0)));

        // Act
        var result = await service.BuildAndPushContainerForDockerfile(dockerfile, builder, imageName, registry, true);

        // Assert
        await shellExecutionService.Received(2).ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null));
        result.Should().BeTrue();
    }

    [Fact]
    public async Task BuildAndPushContainerForDockerfile_ShouldSetEnvVarsAsBuildArgs_WhenCalled()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/testDockerfile", string.Empty);
        var console = new TestConsole();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var dockerfile = new Dockerfile
        {
            Path = "/testDockerfile",
            Context = "testContext",
            Env = new()
            {
                ["TestArg"] = "TestValue",
                ["TestArgTwo"] = "TestValueTwo",
            },
        };

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, "test", string.Empty, 0)));

        // Act
        await service.BuildAndPushContainerForDockerfile(dockerfile, "testBuilder", "testImageName", "testRegistry", true);

        // Assert
        var calls = shellExecutionService.ReceivedCalls().ToArray();
        calls.Length.Should().Be(2);

        var buildCall = calls[0];
        VerifyDockerCall(buildCall, "build --tag \"testRegistry/testImageName:latest\" --build-arg TestArg=\"TestValue\" --build-arg TestArgTwo=\"TestValueTwo\" --file \"/testDockerfile\" testContext");

        var pushCall = calls[1];
        VerifyDockerCall(pushCall, "push testRegistry/testImageName:latest");
    }

    private static void VerifyDockerCall(ICall call, string expectedArgumentsOutput)
    {
        if (call.GetArguments()[0] is not ShellCommandOptions options)
        {
            throw new InvalidOperationException("The shell execution service was not called with the expected arguments.");
        }

        options.Should().NotBeNull();
        options.ArgumentsBuilder.RenderArguments().Should().Be(expectedArgumentsOutput);
    }
}
