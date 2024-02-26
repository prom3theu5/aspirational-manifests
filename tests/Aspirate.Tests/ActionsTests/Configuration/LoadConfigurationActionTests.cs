using Aspirate.Commands.Actions.Configuration;

namespace Aspirate.Tests.ActionsTests.Configuration;

public class LoadConfigurationActionTests : BaseActionTests<LoadConfigurationAction>
{
    [Theory]
    [InlineData(null)]
    [InlineData("/some/path")]
    public async Task ExecuteLoadConfigurationAction_Success(string? path)
    {
        // Arrange
        var state = CreateAspirateState(projectPath: path);
        var serviceProvider = CreateServiceProvider(state);
        var loadConfigurationAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await loadConfigurationAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
    }
}
