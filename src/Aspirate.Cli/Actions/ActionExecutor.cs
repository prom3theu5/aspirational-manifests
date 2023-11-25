namespace Aspirate.Cli.Actions;

public class ActionExecutor(IAnsiConsole console, IServiceProvider serviceProvider)
{
    public static ActionExecutor CreateInstance(IServiceProvider serviceProvider) => new(serviceProvider.GetRequiredService<IAnsiConsole>(), serviceProvider);

    private readonly Queue<ExecutionAction> _actionQueue = new();

    public ActionExecutor QueueAction(string actionKey, Func<Task>? onFailure = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(actionKey, nameof(actionKey));
        _actionQueue.Enqueue(new(actionKey, onFailure));

        return this;
    }

    public async Task<int> ExecuteCommandsAsync()
    {
        while (_actionQueue.Count > 0)
        {
            var executionAction = _actionQueue.Dequeue();
            var action = serviceProvider.GetRequiredKeyedService<IAction>(executionAction.ActionKey);

            try
            {
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
            catch (Exception)
            {
                await HandleActionFailure(executionAction.OnFailure);
                return 1;
            }
        }

        console.MarkupLine($"\r\n[bold] {EmojiLiterals.Rocket} Execution Completed[/]");
        return 0;
    }

    private static Task HandleActionFailure(Func<Task>? onFailure = null) =>
        onFailure != null ? onFailure() : Task.CompletedTask;

    private record ExecutionAction(string ActionKey, Func<Task>? OnFailure = null);
}

