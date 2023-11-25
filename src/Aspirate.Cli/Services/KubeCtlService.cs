namespace Aspirate.Cli.Services;

public class KubeCtlService(IFileSystem filesystem, IAnsiConsole console) : IKubeCtlService
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();

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

        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlApplyArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlKustomizeManifestsArgument, fullOutputPath, quoteValue: false);

        var arguments = argumentsBuilder.RenderArguments();

        var executeCommand = CliWrap.Cli.Wrap(KubeCtlLiterals.KubeCtlCommand)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(_stdErrBuffer));

        await foreach(var cmdEvent in executeCommand.ListenAsync())
        {
            switch (cmdEvent)
            {
                case StartedCommandEvent _:
                    console.WriteLine();
                    console.MarkupLine($"[cyan]Executing: [green]{KubeCtlLiterals.KubeCtlCommand} {arguments}[/] against kubernetes context [blue]{context}.[/][/]");
                    break;
                case StandardOutputCommandEvent stdOut:
                    console.WriteLine(stdOut.Text);
                    break;
                case StandardErrorCommandEvent stdErr:
                    console.MarkupLine($"[red]{stdErr.Text}[/]");
                    break;
                case ExitedCommandEvent exited:
                    if (exited.ExitCode != 0)
                    {
                        console.MarkupLine($"[red]Failed to deploy manifests in [blue]'{fullOutputPath}'[/][/]");
                        throw new ActionCausesExitException(exited.ExitCode);
                    }
                    break;
            }
        }

        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        return true;
    }

    public async Task<bool> RemoveManifests(string context, string outputFolder)
    {
        if (!EnsureActiveContextIsSet(context))
        {
            return false;
        }

        var fullOutputPath = filesystem.GetFullPath(outputFolder);

        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlDeleteArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlKustomizeManifestsArgument, fullOutputPath, quoteValue: false);

        var arguments = argumentsBuilder.RenderArguments();

        var executeCommand = CliWrap.Cli.Wrap(KubeCtlLiterals.KubeCtlCommand)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(_stdErrBuffer));

        await foreach(var cmdEvent in executeCommand.ListenAsync())
        {
            switch (cmdEvent)
            {
                case StartedCommandEvent _:
                    console.WriteLine();
                    console.MarkupLine($"[cyan]Executing: [green]{KubeCtlLiterals.KubeCtlCommand} {arguments}[/] against kubernetes context [blue]{context}.[/][/]");
                    break;
                case StandardOutputCommandEvent stdOut:
                    console.WriteLine(stdOut.Text);
                    break;
                case StandardErrorCommandEvent stdErr:
                    console.MarkupLine($"[red]{stdErr.Text}[/]");
                    break;
                case ExitedCommandEvent exited:
                    if (exited.ExitCode != 0)
                    {
                        console.MarkupLine($"[red]Failed to remove manifests in [blue]'{fullOutputPath}'[/][/]");
                        throw new ActionCausesExitException(exited.ExitCode);
                    }
                    break;
            }
        }

        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        return true;
    }

    private async Task<IReadOnlyCollection<string?>> GatherContexts()
    {
        _stdOutBuffer.Clear();

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlConfigArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlViewArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlOutputArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlOutputJsonArgument, string.Empty, quoteValue: false);

        var arguments = argumentsBuilder.RenderArguments();

        var commandResult = await CliWrap.Cli.Wrap(KubeCtlLiterals.KubeCtlCommand)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .ExecuteAsync();

        if (commandResult.ExitCode != 0)
        {
            console.MarkupLine("[red]Failed to gather Kubernetes contexts from kubeconfig[/]");
        }

        var contexts = ParseResponseAsContextList(_stdOutBuffer.ToString());
        _stdOutBuffer.Clear();

        return contexts;
    }

    private async Task<bool> SetActiveContext(string context)
    {
        if (!EnsureActiveContextIsSet(context))
        {
            return false;
        }

        _stdOutBuffer.Clear();

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(KubeCtlLiterals.KubeCtlConfigArgument, string.Empty, quoteValue: false)
            .AppendArgument(KubeCtlLiterals.KubeCtlUseContextArgument, context, quoteValue: false);

        var arguments = argumentsBuilder.RenderArguments();

        var commandResult = await CliWrap.Cli.Wrap(KubeCtlLiterals.KubeCtlCommand)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .ExecuteAsync();

        if (commandResult.ExitCode != 0)
        {
            console.MarkupLine($"[red]Failed to set Active Kubernetes Context to [blue]'{context}'[/][/]");
            return false;
        }

        console.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Successfully set the Active Kubernetes Context to [blue]'{context}'[/]");
        return true;
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
