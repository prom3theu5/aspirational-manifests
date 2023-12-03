namespace Aspirate.Services.Implementations;

public class KubeCtlService(IFileSystem filesystem, IAnsiConsole console, IShellExecutionService shellExecutionService) : IKubeCtlService
{
    public async Task<string?> SelectKubernetesContextForDeployment()
    {
        var contexts = await GatherContexts();

        if (contexts.Count == 0)
        {
            console.MarkupLine("[red]No Kubernetes contexts found in kubeconfig[/]");
            return null;
        }

        var activeContextName = SelectKubernetesContextToUse(contexts!);

        var successfullySet = await SetActiveContext(activeContextName);

        return successfullySet ? activeContextName : null;
    }

    public async Task<bool> ApplyManifests(string context, string outputFolder)
    {
        if (!EnsureActiveContextIsSet(context))
        {
            return false;
        }

        var fullOutputPath = filesystem.GetFullPath(outputFolder);

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlApplyArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlKustomizeManifestsArgument, fullOutputPath, quoteValue: false);

        var executionOptions = new ShellCommandOptions
        {
            Command = KubeCtlLiterals.KubeCtlCommand,
            ArgumentsBuilder = argumentsBuilder,
            PreCommandMessage =
                $"[cyan]Executing: [green]{KubeCtlLiterals.KubeCtlCommand} {argumentsBuilder.RenderArguments()}[/] against kubernetes context [blue]{context}.[/][/]",
            FailureCommandMessage = $"[red]Failed to deploy manifests in [blue]'{fullOutputPath}'[/][/]",
            ShowOutput = true,
        };

        _ = await shellExecutionService.ExecuteCommand(executionOptions);

        return true;
    }

    public async Task<bool> RemoveManifests(string context, string outputFolder)
    {
        if (!EnsureActiveContextIsSet(context))
        {
            return false;
        }

        var fullOutputPath = filesystem.GetFullPath(outputFolder);

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlDeleteArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlKustomizeManifestsArgument, fullOutputPath, quoteValue: false);

        var executionOptions = new ShellCommandOptions
        {
            Command = KubeCtlLiterals.KubeCtlCommand,
            ArgumentsBuilder = argumentsBuilder,
            PreCommandMessage = $"[cyan]Executing: [green]{KubeCtlLiterals.KubeCtlCommand} {argumentsBuilder.RenderArguments()}[/] against kubernetes context [blue]{context}.[/][/]",
            FailureCommandMessage = $"[red]Failed to remove manifests in [blue]'{fullOutputPath}'[/][/]",
            ShowOutput = true,
        };

        _ = await shellExecutionService.ExecuteCommand(executionOptions);

        return true;
    }

    private async Task<IReadOnlyCollection<string?>> GatherContexts()
    {
        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlConfigArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlViewArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlOutputArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlOutputJsonArgument, string.Empty, quoteValue: false);

        var executionOptions = new ShellCommandOptions
        {
            Command = KubeCtlLiterals.KubeCtlCommand,
            ArgumentsBuilder = argumentsBuilder,
            FailureCommandMessage = "[red]Failed to gather Kubernetes contexts from kubeconfig[/]",
        };

        var contextOutput = await shellExecutionService.ExecuteCommand(executionOptions);

        return ParseResponseAsContextList(contextOutput.Output);
    }

    private async Task<bool> SetActiveContext(string context)
    {
        if (!EnsureActiveContextIsSet(context))
        {
            return false;
        }

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlConfigArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlUseContextArgument, context, quoteValue: false);

        var executionOptions = new ShellCommandOptions
        {
            Command = KubeCtlLiterals.KubeCtlCommand,
            ArgumentsBuilder = argumentsBuilder,
            FailureCommandMessage = $"[red]Failed to set Active Kubernetes Context to [blue]'{context}'[/][/]",
            SuccessCommandMessage = $"[green]({EmojiLiterals.CheckMark}) Done:[/] Successfully set the Active Kubernetes Context to [blue]'{context}'[/]",
        };

        var result = await shellExecutionService.ExecuteCommand(executionOptions);

        return result.Success;
    }

    private bool EnsureActiveContextIsSet(string context)
    {
        if (string.IsNullOrEmpty(context))
        {
            console.MarkupLine("[red]Active context has not been set.[/]");
            return false;
        }

        return true;
    }

    private static List<string?> ParseResponseAsContextList(string jsonString)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(jsonString);
            var root = jsonDoc.RootElement;
            var contexts = root.GetProperty(KubeCtlLiterals.KubeCtlContextsProperty);

            return contexts.EnumerateArray()
                .Select(context => context.GetProperty(KubeCtlLiterals.KubeCtlNameProperty).GetString())
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    private string SelectKubernetesContextToUse(IReadOnlyCollection<string> contextNames) =>
        console.Prompt(
            new SelectionPrompt<string>()
                .Title("Select [green]kubernetes context[/] to use for deployment")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more contexts)[/]")
                .AddChoices(contextNames));
}
