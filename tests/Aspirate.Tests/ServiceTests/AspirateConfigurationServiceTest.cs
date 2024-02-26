namespace Aspirate.Tests.ServiceTests;

public class AspirateConfigurationServiceTest
{
    private const string AppHostPath = "/some-path";
    private readonly IAnsiConsole _console = new TestConsole();

    [Fact]
    public async Task LoadConfigurationFile_ReturnsExpectedObject_WhenConfigurationFileExists()
    {
        // Arrange
        var testFileSystem = SetupTestFilesystem();
        var configurationService = new AspirateConfigurationService(_console, testFileSystem);

        // Act
        var actualSettings = configurationService.LoadConfigurationFile(AppHostPath);

        // Assert
        await Verify(actualSettings)
            .UseDirectory("VerifyResults");
    }

    private static MockFileSystem SetupTestFilesystem()
    {
        var expectedSettings = new AspirateSettings
        {
            TemplatePath = "SomeTemplatePath",
            ContainerSettings = new()
            {
                Registry = "SomeRegistry",
                Tag = "SomeTag",
            },
        };

        var testFileSystem = new MockFileSystem();
        testFileSystem.AddDirectory(AppHostPath);
        var configurationPath = testFileSystem.NormalizePath(AppHostPath);
        var configurationFile = testFileSystem.Path.Combine(configurationPath, AspirateSettings.FileName);
        testFileSystem.AddFile(configurationFile, JsonSerializer.Serialize(expectedSettings));

        return testFileSystem;
    }
}
