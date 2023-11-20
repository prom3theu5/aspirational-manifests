namespace Aspirate.Cli.Services;

public class ProjectPropertyService : IProjectPropertyService
{
    private readonly StringBuilder _stdOutBuffer = new();

    public async Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames)
    {
        var workingDirectory = Path.GetDirectoryName(projectPath) ?? throw new($"Could not get directory name from {projectPath}");
        var propertyValues = await ExecuteDotnetMsBuildGetPropertyCommand(workingDirectory, propertyNames);

        return propertyValues ?? null;
    }

    private async Task<string?> ExecuteDotnetMsBuildGetPropertyCommand(string workingDirectory, params string[] propertyNames)
    {
        _stdOutBuffer.Clear();

        var executionCommand = CliWrap.Cli.Wrap("dotnet")
            .WithArguments("msbuild");

        executionCommand = propertyNames.Aggregate(executionCommand, (current, propertyName) => current.WithArguments($"-p:{propertyName}"));

        var commandResult = await executionCommand.WithWorkingDirectory(workingDirectory)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(_stdOutBuffer))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        return commandResult.ExitCode != 0 ? null : _stdOutBuffer.ToString();
    }

}
