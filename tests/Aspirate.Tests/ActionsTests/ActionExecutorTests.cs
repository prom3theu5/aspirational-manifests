namespace Aspirate.Tests.ActionsTests;

public class ActionExecutorTests
{
    private readonly IAnsiConsole _console = new TestConsole();
    private readonly IServiceProvider _serviceProvider;
    private readonly IAction _action = Substitute.For<IAction>();
    private readonly AspirateState _state = new();

    public ActionExecutorTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IAction>("testAction", _action);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task ExecuteCommandsAsync_ShouldCallExpectedMethods()
    {
        // Arrange
        _action.ClearSubstitute();
        _action.ExecuteAsync().Returns(Task.FromResult(true));
        var actionExecutor = new ActionExecutor(_console, _serviceProvider, _state);

        // Act
        var result = await actionExecutor.QueueAction("testAction").ExecuteCommandsAsync();

        // Assert
        result.Should().Be(0);
        await _action.Received(1).ExecuteAsync();
    }

    [Fact]
    public async Task ExecuteCommandsAsync_WhenActionThrowsExitException_ShouldReturnExitCode()
    {
        // Arrange
        _action.ClearSubstitute();
        _action.ExecuteAsync().Throws(new ActionCausesExitException(2));
        var actionExecutor = new ActionExecutor(_console, _serviceProvider, _state);

        // Act
        var result = await actionExecutor.QueueAction("testAction").ExecuteCommandsAsync();

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task ExecuteCommandsAsync_WhenActionThrowsException_ShouldReturnOne()
    {
        // Arrange
        _action.ClearSubstitute();
        _action.ExecuteAsync().Throws(new Exception());
        var actionExecutor = new ActionExecutor(_console, _serviceProvider, _state);

        // Act
        var result = await actionExecutor.QueueAction("testAction").ExecuteCommandsAsync();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteCommandsAsync_WhenActionFails_ShouldReturnOne()
    {
        // Arrange
        _action.ClearSubstitute();
        _action.ExecuteAsync().Returns(Task.FromResult(false));
        var actionExecutor = new ActionExecutor(_console, _serviceProvider, _state);

        // Act
        var result = await actionExecutor.QueueAction("testAction").ExecuteCommandsAsync();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteCommandsAsync_WhenStateIsNonInteractive_ShouldReturnZeroAndPrintMessage()
    {
        // Arrange
        _action.ClearSubstitute();
        _action.ExecuteAsync().Returns(Task.FromResult(true));
        _state.NonInteractive = true;
        var actionExecutor = new ActionExecutor(_console, _serviceProvider, _state);

        // Act
        var result = await actionExecutor.QueueAction("testAction").ExecuteCommandsAsync();

        // Assert
        result.Should().Be(0);
        var outputConsole = _console as TestConsole;
        outputConsole.Output.Should().Contain("Non-interactive mode enabled.");
    }

    [Fact]
    public async Task ExecuteCommandsAsync_WhenActionQueueIsEmpty_ShouldReturnZeroAndPrintMessage()
    {
        // Arrange
        var actionExecutor = new ActionExecutor(_console, _serviceProvider, _state);

        // Act
        var result = await actionExecutor.ExecuteCommandsAsync();

        // Assert
        result.Should().Be(0);
        var outputConsole = _console as TestConsole;
        outputConsole.Output.Should().Contain("Execution Completed");
    }
}
