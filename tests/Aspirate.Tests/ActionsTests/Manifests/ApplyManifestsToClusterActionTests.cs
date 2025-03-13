using System;
using System.Threading.Tasks;
using Xunit;

namespace Aspirate.Tests.ActionsTests.Manifests;

public class ApplyManifestsToClusterActionTests : BaseActionTests<ApplyManifestsToClusterAction>
{
    private const string ContextsResponse =
        """
        {
          "contexts": [
            {
              "name": "docker-desktop",
              "context": {
                "cluster": "docker-desktop",
                "user": "docker-desktop"
              }
            },
            {
              "name": "experiments",
              "context": {
                "cluster": "experiments",
                "user": "experiments"
              }
            }
          ],
          "current-context": "docker-desktop"
        }
        """;

    [Fact]
    public async Task ExecuteApplyManifestsToClusterActionTests_Interactive_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("y");
        console.Input.PushKey(ConsoleKey.Enter);
        var state = CreateAspirateState(projectPath: DefaultProjectPath, inputPath: "/some-path");
        var serviceProvider = CreateServiceProvider(state, console, new FileSystem());

        var mockExecutorService = serviceProvider.GetRequiredService<IShellExecutionService>();

        mockExecutorService.ClearSubstitute();

        mockExecutorService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .ReturnsForAnyArgs(new ShellCommandResult(true, ContextsResponse, string.Empty, 0));

        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await generateAspireManifestAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateApplyManifestsToClusterActionTests_NonInteractiveNoContext_ThrowsActionCausesExitException()
    {
        // Arrange
        var state = CreateAspirateState(nonInteractive: true, projectPath: DefaultProjectPath, inputPath: "/some-path");
        var serviceProvider = CreateServiceProvider(state, fileSystem: new FileSystem());
        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => generateAspireManifestAction.ValidateNonInteractiveState();

        // Assert
        act.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void ValidateApplyManifestsToClusterActionTests_NonInteractiveContextSet_DoesNotThrow()
    {
        // Arrange
        var state = CreateAspirateState(nonInteractive: true, projectPath: DefaultProjectPath, inputPath: "/some-path", kubeContext: "docker-desktop");
        var serviceProvider = CreateServiceProvider(state, fileSystem: new FileSystem());
        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => generateAspireManifestAction.ValidateNonInteractiveState();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task ExecuteApplyManifestsToClusterActionTests_NonInteractive_Success()
    {
        // Arrange
        var state = CreateAspirateState(nonInteractive: true, projectPath: DefaultProjectPath, inputPath: "/some-path", kubeContext: "docker-desktop");
        var serviceProvider = CreateServiceProvider(state, fileSystem: new FileSystem());

        var mockExecutorService = serviceProvider.GetRequiredService<IShellExecutionService>();

        mockExecutorService.ClearSubstitute();

        mockExecutorService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .ReturnsForAnyArgs(new ShellCommandResult(true, ContextsResponse, string.Empty, 0));

        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await generateAspireManifestAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
    }
}
