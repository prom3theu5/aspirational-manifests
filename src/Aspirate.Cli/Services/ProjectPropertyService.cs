namespace Aspirate.Cli.Services;

public sealed class ProjectPropertyService(IFileSystem filesystem) : IProjectPropertyService
{
    private readonly StringBuilder _stdOutBuffer = new();

    public async Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames)
    {
        var currentDirectory = filesystem.Directory.GetCurrentDirectory();
        var normalizedProjectPath = projectPath.Replace('\\', filesystem.Path.DirectorySeparatorChar);
        var fullProjectPath = filesystem.Path.Combine(currentDirectory, normalizedProjectPath);
        var projectDirectory = filesystem.Path.GetDirectoryName(fullProjectPath) ?? throw new($"Could not get directory name from {fullProjectPath}");
        var propertyValues = await ExecuteDotnetMsBuildGetPropertyCommand(projectDirectory, propertyNames);

        return propertyValues ?? null;
    }

    private async Task<string?> ExecuteDotnetMsBuildGetPropertyCommand(string workingDirectory, params string[] propertyNames)
    {
        _stdOutBuffer.Clear();

        var arguments = new List<string>
        {
            "msbuild",
        };

        arguments.AddRange(propertyNames.Select(propertyName => $"--getProperty:{propertyName}"));

        var executionCommand = CliWrap.Cli.Wrap("dotnet")
            .WithArguments(arguments);

        var commandResult = await executionCommand.WithWorkingDirectory(workingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        return commandResult.ExitCode != 0 ? null : _stdOutBuffer.ToString();
    }
}
