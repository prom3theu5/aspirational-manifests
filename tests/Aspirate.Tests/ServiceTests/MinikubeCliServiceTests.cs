namespace Aspirate.Tests.ServiceTests;

public class TestableMinikubeCliService(
    IShellExecutionService shellExecutionService,
    IAnsiConsole logger,
    IProcessService processService
) : MinikubeCliService(shellExecutionService, logger, processService)
{
}

public class MinikubeCliServiceTests
{
    [Fact]
    public async Task ActivateAndKillMinikubeMount_ShouldSimulateProcessExecutionAndDestruction()
    {
        // Arrange
        var logger = Substitute.For<IAnsiConsole>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();
        var processService = new FakeProcessService(false);

        var minikubeCliService = new MinikubeCliService(shellExecutionService, logger, processService)
        {
            DefaultDelay = 2000
        };

        var state = new AspirateState
        {
            BindMounts = new Dictionary<string, Dictionary<string, int>>
            {
                { "/target-path1", new Dictionary<string, int> { { "/source-path1", 0 } } },
                { "/target-path2", new Dictionary<string, int> { { "/source-path2", 0 } } },
                { "/target-path3", new Dictionary<string, int> { { "/source-path3", 0 } } }
            }
        };

        shellExecutionService.ExecuteCommand(Arg.Any<ShellCommandOptions>())
            .Returns(Task.FromResult(new ShellCommandResult(true, "Mocked Output", "", 0)));

        // Act
        await minikubeCliService.ActivateMinikubeMount(state);

        foreach (var mount in state.BindMounts)
        {
            var entry = mount.Value;
            Assert.True(entry.Count > 0);

            foreach (var kvp in entry)
            {
                Assert.True(kvp.Value >= 1000, $"Process ID should be assigned for {kvp.Key}");
            }
        }

        var result = await minikubeCliService.KillMinikubeMounts(state);
        Assert.True(result);
    }

    [Fact]
    public async Task ActivateAndKillMinikubeMount_ShouldSimulateProcessExecutionAndDestruction_WithChocolateyShimgen()
    {
        // Arrange
        var logger = Substitute.For<IAnsiConsole>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();
        var processService = new FakeProcessService(true);

        var minikubeCliService = new MinikubeCliService(shellExecutionService, logger, processService)
        {
            DefaultDelay = 2000
        };

        var state = new AspirateState
        {
            BindMounts = new Dictionary<string, Dictionary<string, int>>
            {
                { "/target-path1", new Dictionary<string, int> { { "/source-path1", 0 } } },
                { "/target-path2", new Dictionary<string, int> { { "/source-path2", 0 } } },
                { "/target-path3", new Dictionary<string, int> { { "/source-path3", 0 } } }
            }
        };

        shellExecutionService.ExecuteCommand(Arg.Any<ShellCommandOptions>())
            .Returns(Task.FromResult(new ShellCommandResult(true, "Mocked Output", "", 0)));

        // Act
        await minikubeCliService.ActivateMinikubeMount(state);

        foreach (var mount in state.BindMounts)
        {
            var entry = mount.Value;
            Assert.True(entry.Count > 0);

            foreach (var kvp in entry)
            {
                Assert.True(kvp.Value >= 1000, $"Process ID should be assigned for {kvp.Key}");
            }
        }

        var result = await minikubeCliService.KillMinikubeMounts(state);
        Assert.True(result);
    }

    [Fact]
    public async Task ActivateMinikubeMount_ProcessIdsShouldBeUnique()
    {
        // Arrange
        var set = new HashSet<int>();
        var logger = Substitute.For<IAnsiConsole>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();
        var processService = new FakeProcessService(false);

        var minikubeCliService = new MinikubeCliService(shellExecutionService, logger, processService)
        {
            DefaultDelay = 2000
        };

        var state = new AspirateState
        {
            BindMounts = new Dictionary<string, Dictionary<string, int>>
            {
                { "/target-path1", new Dictionary<string, int> { { "/source-path1", 0 } } },
                { "/target-path2", new Dictionary<string, int> { { "/source-path2", 0 } } },
                { "/target-path3", new Dictionary<string, int> { { "/source-path3", 0 } } }
            }
        };

        shellExecutionService.ExecuteCommand(Arg.Any<ShellCommandOptions>())
            .Returns(Task.FromResult(new ShellCommandResult(true, "Mocked Output", "", 0)));

        // Act
        await minikubeCliService.ActivateMinikubeMount(state);

        int count = 0;
        foreach (var mount in state.BindMounts)
        {
            var entry = mount.Value;

            foreach (var kvp in entry)
            {
                count++;
                set.Add(kvp.Value);
            }
        }

        Assert.Equal(count, set.Count);
    }

    [Fact]
    public async Task ActivateMinikubeMount_ShouldHandleFailedProcessStart()
    {
        // Arrange
        var logger = Substitute.For<IAnsiConsole>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var processService = Substitute.For<IProcessService>();
        processService.StartProcess(Arg.Any<ProcessStartInfo>()).Returns((ProcessWrapper)null!);

        var minikubeCliService = new MinikubeCliService(shellExecutionService, logger, processService)
        {
            DefaultDelay = 2000
        };

        var state = new AspirateState
        {
            BindMounts = new Dictionary<string, Dictionary<string, int>>
        {
            { "/test-path", new Dictionary<string, int> { { "/container-path", 0 } } }
        }
        };

        // Act
        await minikubeCliService.ActivateMinikubeMount(state);

        // Assert
        foreach (var target in state.BindMounts["/test-path"])
        {
            Assert.Equal(0, target.Value);
        }
    }

    [Fact]
    public async Task KillMinikubeMounts_ShouldReturnTrue_WhenNoProcessesToKill()
    {
        // Arrange
        var logger = Substitute.For<IAnsiConsole>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();
        var processService = new FakeProcessService(false);

        var minikubeCliService = new MinikubeCliService(shellExecutionService, logger, processService);

        var state = new AspirateState { BindMounts = new Dictionary<string, Dictionary<string, int>>() };

        // Act
        var result = await minikubeCliService.KillMinikubeMounts(state);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task KillMinikubeMounts_ShouldHandleInvalidProcessIdsGracefully()
    {
        // Arrange
        var logger = Substitute.For<IAnsiConsole>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();
        var processService = Substitute.For<IProcessService>();

        processService.GetProcessById(Arg.Any<int>()).Returns((ProcessWrapper)null!);
        processService.KillProcess(Arg.Any<int>()).Returns(Task.FromResult(false));

        var minikubeCliService = new MinikubeCliService(shellExecutionService, logger, processService);

        var state = new AspirateState
        {
            BindMounts = new Dictionary<string, Dictionary<string, int>>
        {
            { "/test-path", new Dictionary<string, int> { { "/container-path", 99999 } } }
        }
        };

        // Act
        var result = await minikubeCliService.KillMinikubeMounts(state);

        // Assert
        Assert.False(result);
    }
}

public class FakeProcessService(bool testChocolatey) : IProcessService
{
    private static int _nextId = 1000;
    private static int _childId = 2000;

    private const string MinikubeFilePath = @"C:\ProgramData\minikube\minikube.exe";
    private const string ChocolateyFilePath = @"C:\ProgramData\chocolatey\bin\minikube.exe";

    private readonly List<ProcessWrapper> _fakeProcesses = new();

    public ProcessWrapper StartProcess(ProcessStartInfo startInfo)
    {
        int processId = _nextId++;
        int childProcessId = _childId++;

        string fileName = testChocolatey ? ChocolateyFilePath : MinikubeFilePath;
        string childFileName = MinikubeFilePath;

        var process = new ProcessWrapper(processId, fileName);
        _fakeProcesses.Add(process);

        if (testChocolatey)
        {
            _fakeProcesses.Add(new(childProcessId, childFileName, processId));
        }

        return process;
    }

    public async Task<bool> KillProcess(int processId)
    {
        bool killedAll = true;

        if (IsChocolateyProcess(processId))
        {
            killedAll = await KillChildProcessesByParentId(processId);
        }

        var process = _fakeProcesses.FirstOrDefault(p => p.Id == processId);

        if (process != null)
        {
            if (!_fakeProcesses.Remove(process))
            {
                killedAll = false;
            }
        }

        return killedAll;
    }
    public async Task<bool> KillChildProcessesByParentId(int processId)
    {
        var children = _fakeProcesses.Where(p => p.ParentId == processId).ToList();

        await Task.CompletedTask;

        return children.All(_fakeProcesses.Remove);
    }

    public bool IsChocolateyProcess(int processId)
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        try
        {
            string processPath = GetProcessPath(processId);
            return processPath.Contains("chocolatey");
        }
        catch
        {
        }
        return false;
    }

    public string GetProcessPath(int processId)
    {
        var process = _fakeProcesses.First(p => p.Id == processId);

        return process?.FileName ?? "Unknown";
    }
    public ProcessWrapper? GetProcessById(int processId) => _fakeProcesses.First(p => p.Id == processId);
}
