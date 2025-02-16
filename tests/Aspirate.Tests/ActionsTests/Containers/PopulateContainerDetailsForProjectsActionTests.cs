using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Aspirate.Tests.ActionsTests.Containers;

public class PopulateContainerDetailsForProjectsActionTests : BaseActionTests<PopulateContainerDetailsForProjectsAction>
{
    private const string PropertiesResponse =
        """
        {
          "Properties": {
            "ContainerRegistry": "localhost:5001",
            "ContainerRepository": "",
            "ContainerImageName": "",
            "ContainerImageTag": ""
          }
        }
        """;

    [Fact]
    public async Task ExecutePopulateContainerDetailsForProjectsAction_NonInteractive_Success()
    {
        // Arrange
        var aspireManifest = Path.Combine(AppContext.BaseDirectory, "TestData", "sqlserver-endtoend.json");
        var state = CreateAspirateState(nonInteractive: true, aspireManifest: aspireManifest);
        var serviceProvider = CreateServiceProvider(state, fileSystem: new FileSystem());
        var loadManifestAction =
            serviceProvider.GetRequiredKeyedService<IAction>(nameof(LoadAspireManifestAction)) as LoadAspireManifestAction;

        _ = await loadManifestAction.ExecuteAsync();

        var mockExecutorService = serviceProvider.GetRequiredService<IShellExecutionService>();

        mockExecutorService.ClearSubstitute();

        mockExecutorService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .ReturnsForAnyArgs(new ShellCommandResult(true, PropertiesResponse, string.Empty, 0));

        var populateContainerDetailsForProjectsAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await populateContainerDetailsForProjectsAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
    }
}
