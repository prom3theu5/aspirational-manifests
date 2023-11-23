namespace Aspirate.Cli.Services;

public class AspireManifestCompositionService(IFileSystem fileSystem, IAnsiConsole console) : IAspireManifestCompositionService
{
    private readonly StringBuilder _stdOutBuffer = new();
    private readonly StringBuilder _stdErrBuffer = new();

    public async Task<(bool Success, string FullPath)> BuildManifestForProject(string appHostProject)
    {
        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        if (appHostProject.StartsWith('.'))
        {
            var currentDirectory = fileSystem.Directory.GetCurrentDirectory();
            var normalizedProjectPath = appHostProject.Replace('\\', fileSystem.Path.DirectorySeparatorChar);
            appHostProject = fileSystem.Path.Combine(currentDirectory, normalizedProjectPath);
        }

        var outputFile = await BuildManifest(appHostProject);

        return (true, outputFile);
    }

    private async Task<string> BuildManifest(string projectPath, string outputFileName = "manifest.json")
    {
        var appHostDirectory = fileSystem.Path.GetDirectoryName(projectPath);
        var outputFile = fileSystem.Path.Combine(appHostDirectory, outputFileName);

        if (fileSystem.File.Exists(outputFile))
        {
            fileSystem.File.Delete(outputFile);
        }

        var arguments = AspireLiterals.BuildManifestCommand(projectPath, outputFile);

        var executeCommand = CliWrap.Cli.Wrap("dotnet")
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
                        Environment.Exit(exited.ExitCode);
                    }
                    break;
            }
        }

        _stdErrBuffer.Clear();
        _stdOutBuffer.Clear();

        return outputFile;
    }
}
