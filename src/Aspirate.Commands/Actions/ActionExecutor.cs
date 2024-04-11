namespace Aspirate.Commands.Actions;

/// <summary>
/// Represents a class responsible for executing a queue of actions.
/// </summary>
public class ActionExecutor(IAnsiConsole console, IServiceProvider serviceProvider, AspirateState state)
{
    /// <summary>
    /// Creates an instance of ActionExecutor using the provided IServiceProvider.
    /// </summary>
    /// <param name="serviceProvider">The IServiceProvider used to retrieve required services.</param>
    /// <returns>An instance of ActionExecutor.</returns>
    public static ActionExecutor CreateInstance(IServiceProvider serviceProvider) =>
        new(serviceProvider.GetRequiredService<IAnsiConsole>(), serviceProvider, serviceProvider.GetRequiredService<AspirateState>());

    /// <summary>
    /// Represents a queue of <see cref="ExecutionAction"/> objects for delayed execution.
    /// </summary>
    private readonly Queue<ExecutionAction> _actionQueue = new();

    /// <summary>
    /// Queues an action to be executed.
    /// </summary>
    /// <param name="actionKey">The key of the action to be executed.</param>
    /// <param name="onFailure">An optional action to be executed if the queued action fails.</param>
    /// <returns>
    /// The <see cref="ActionExecutor"/> instance to allow method chaining.
    /// </returns>
    public ActionExecutor QueueAction(string actionKey, Func<Task>? onFailure = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(actionKey, nameof(actionKey));
        _actionQueue.Enqueue(new(actionKey, onFailure));

        return this;
    }

    /// <summary>
    /// Executes a queue of actions asynchronously.
    /// </summary>
    /// <returns>
    /// An integer value representing the execution result:
    /// - 0 if all actions were successfully executed.
    /// - 1 if any action failed to execute.
    /// - The exit code if an action caused the program to exit.
    /// </returns>
    public async Task<int> ExecuteCommandsAsync()
    {
        if (state.NonInteractive)
        {
            console.MarkupLine("[blue]Non-interactive mode enabled.[/]");
        }

        while (_actionQueue.Count > 0)
        {
            var executionAction = _actionQueue.Dequeue();
            var action = serviceProvider.GetRequiredKeyedService<IAction>(executionAction.ActionKey);

            try
            {
                if (state.NonInteractive && action is BaseActionWithNonInteractiveValidation nonInteractiveAction)
                {
                    nonInteractiveAction.ValidateNonInteractiveState();
                }

                var successfullyCompleted = await action.ExecuteAsync();

                if (successfullyCompleted)
                {
                    continue;
                }

                await HandleActionFailure(executionAction.OnFailure);
                return 1;
            }
            catch (ActionCausesExitException exitException)
            {
                // Do nothing - the action is planned, and will skip the rest of the queue, returning the exit code.
                console.MarkupLine($"[red bold]({exitException.ExitCode}): Aspirate will now exit.[/]");
                return exitException.ExitCode;
            }
            catch (Exception ex)
            {
                console.MarkupLine($"[red bold]Error executing action [blue]'{executionAction.ActionKey}'[/]:[/]");
                console.WriteException(ex);
                await HandleActionFailure(executionAction.OnFailure);
                return 1;
            }
        }

        console.WriteLine();
        console.MarkupLine($"[bold] {EmojiLiterals.Rocket} Execution Completed {EmojiLiterals.Rocket}[/]");
        return 0;
    }

    private static Task HandleActionFailure(Func<Task>? onFailure = null) =>
        onFailure != null ? onFailure() : Task.CompletedTask;

    /// <summary>
    /// Represents an action to be executed with optional failure handling. </summary>
    /// /
    private record ExecutionAction(string ActionKey, Func<Task>? OnFailure = null);
}

