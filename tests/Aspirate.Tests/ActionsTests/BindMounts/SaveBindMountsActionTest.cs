using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspirate.Tests.ActionsTests.BindMounts;
public class SaveBindMountsActionTest : BaseActionTests<SaveBindMountsAction>
{
    private const string AppHostPath = $"{DefaultProjectPath}/source/repos/MyRepo/Aspire.AppHost";
    private const string MountPath1 = $"{DefaultProjectPath}/folder/folder/cert";
    private const string MountPath2 = $"{DefaultProjectPath}/folder/folder/temp/something";

    [Fact]
    public async Task ExecuteAsync_InInteractiveMode_ReturnsCorrectResult()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        fileSystem.Directory.CreateDirectory(AppHostPath);
        fileSystem.Directory.CreateDirectory(MountPath1);
        fileSystem.Directory.CreateDirectory(MountPath2);
        fileSystem.Directory.SetCurrentDirectory(AppHostPath);

        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        EnterPasswordInput(console, "secret_password"); // postgrescontainer

        var state = CreateAspirateStateWithBindMounts();
        state.KubeContext = MinikubeLiterals.Path;
        state.EnableMinikubeMountAction = true;

        var serviceProvider = CreateServiceProvider(state, console, fileSystem);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();

        Assert.Equal(2, state.BindMounts.Count);

        string key1 = fileSystem.GetFullPath("../../../../folder/folder/cert");
        string key2 = fileSystem.GetFullPath("../../../../folder/folder/temp/something");

        Assert.True(state.BindMounts.ContainsKey(key1));
        Assert.True(state.BindMounts.ContainsKey(key2));

        string valueKey1 = "/test/.aspnet/https";
        string valueKey2 = "/localdev/.aspnet/https";

        Assert.True(state.BindMounts[key1].ContainsKey(valueKey1));
        Assert.True(state.BindMounts[key2].ContainsKey(valueKey2));

        Assert.Equal(0, state.BindMounts[key1][valueKey1]);
        Assert.Equal(0, state.BindMounts[key2][valueKey2]);
    }

    [Fact]
    public async Task ExecuteAsync_NonInteractiveMode_ReturnsCorrectResult()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        fileSystem.Directory.CreateDirectory(AppHostPath);
        fileSystem.Directory.CreateDirectory(MountPath1);
        fileSystem.Directory.CreateDirectory(MountPath2);
        fileSystem.Directory.SetCurrentDirectory(AppHostPath);

        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        var state = CreateAspirateStateWithBindMounts(nonInteractive: true);
        state.KubeContext = MinikubeLiterals.Path;
        state.EnableMinikubeMountAction = true;

        var serviceProvider = CreateServiceProvider(state, console, fileSystem);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();

        Assert.Equal(2, state.BindMounts.Count);

        string key1 = fileSystem.GetFullPath("../../../../folder/folder/cert");
        string key2 = fileSystem.GetFullPath("../../../../folder/folder/temp/something");

        Assert.True(state.BindMounts.ContainsKey(key1));
        Assert.True(state.BindMounts.ContainsKey(key2));

        string valueKey1 = "/test/.aspnet/https";
        string valueKey2 = "/localdev/.aspnet/https";

        Assert.True(state.BindMounts[key1].ContainsKey(valueKey1));
        Assert.True(state.BindMounts[key2].ContainsKey(valueKey2));

        Assert.Equal(0, state.BindMounts[key1][valueKey1]);
        Assert.Equal(0, state.BindMounts[key2][valueKey2]);

    }

    private static void EnterPasswordInput(TestConsole console, string password)
    {
        // first entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);

        // confirmation entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);
    }
}
