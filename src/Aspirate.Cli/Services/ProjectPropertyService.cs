namespace Aspirate.Cli.Services;

public sealed class ProjectPropertyService(IFileSystem filesystem, ILogger<ProjectPropertyService> logger) : IProjectPropertyService
{
    private readonly StringBuilder _stdOutBuffer = new();

    public async Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames)
    {
        var projectDirectory = filesystem.Path.GetDirectoryName(projectPath) ?? throw new($"Could not get directory name from {projectPath}");
        var propertyValues = await ExecuteDotnetMsBuildGetPropertyCommand(projectDirectory, propertyNames);

        return propertyValues ?? null;
    }

    private async Task<string?> ExecuteDotnetMsBuildGetPropertyCommand(string workingDirectory, params string[] propertyNames)
    {
        _stdOutBuffer.Clear();

        var arguments = new List<string>()
        {
            "msbuild"
        };

        foreach (var propertyName in propertyNames)
        {
            arguments.Add($"--getProperty:{propertyName}");
        }

        var executionCommand = CliWrap.Cli.Wrap("dotnet")
            .WithArguments(arguments);

        var commandResult = await executionCommand.WithWorkingDirectory(workingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        return commandResult.ExitCode != 0 ? null : _stdOutBuffer.ToString();
    }
}
