namespace Aspirate.Cli.Services;

public class AspireManifestCompositionService(IFileSystem fileSystem, IAnsiConsole console) : IAspireManifestCompositionService
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();

    public async Task<(bool Success, string FullPath)> BuildManifestForProject(string appHostProject)
    {
        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        var normalizedPath = fileSystem.NormalizePath(appHostProject);

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(DotNetSdkLiterals.RunArgument, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkLiterals.ProjectArgument, normalizedPath)
            .AppendArgument(DotNetSdkLiterals.ArgumentDelimiter, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkLiterals.PublisherArgument, AspireLiterals.ManifestPublisherArgument, quoteValue: false)
            .AppendArgument(DotNetSdkLiterals.OutputPathArgument, AspireLiterals.DefaultManifestFile, quoteValue: false);

        var outputFile = await BuildManifest(normalizedPath, argumentsBuilder);

        return (true, outputFile);
    }

    private async Task<string> BuildManifest(string projectPath, ArgumentsBuilder argumentsBuilder)
    {
        var appHostDirectory = fileSystem.Path.GetDirectoryName(projectPath);
        var outputFile = fileSystem.Path.Combine(appHostDirectory, AspireLiterals.DefaultManifestFile);

        if (fileSystem.File.Exists(outputFile))
        {
            fileSystem.File.Delete(outputFile);
        }

        var arguments = argumentsBuilder.RenderArguments();

        var executeCommand = CliWrap.Cli.Wrap(DotNetSdkLiterals.DotNetCommand)
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
                    console.MarkupLine($"[cyan]Executing: dotnet {arguments}[/]");
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
                        console.MarkupLine($"[red]{_stdErrBuffer.Append(_stdOutBuffer)}[/]");
                        throw new ActionCausesExitException(exited.ExitCode);
                    }
                    break;
            }
        }

        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        return outputFile;
    }
}
