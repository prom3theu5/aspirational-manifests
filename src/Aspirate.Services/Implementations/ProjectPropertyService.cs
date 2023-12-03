namespace Aspirate.Services.Implementations;

public sealed class ProjectPropertyService(IFileSystem filesystem, IShellExecutionService shellExecutionService) : IProjectPropertyService
{
    public async Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames)
    {
        var fullProjectPath = filesystem.NormalizePath(projectPath);
        var projectDirectory = filesystem.Path.GetDirectoryName(fullProjectPath);
        var propertyValues = await ExecuteDotnetMsBuildGetPropertyCommand(projectDirectory, propertyNames);

        return propertyValues ?? null;
    }

    private async Task<string?> ExecuteDotnetMsBuildGetPropertyCommand(string workingDirectory, params string[] propertyNames)
    {
        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(DotNetSdkLiterals.MsBuildArgument, string.Empty, quoteValue: false);

        foreach (var propertyName in propertyNames)
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.GetPropertyArgument, propertyName, true);
        }

        var result = await shellExecutionService.ExecuteCommand(new()
        {
            Command = DotNetSdkLiterals.DotNetCommand,
            ArgumentsBuilder = argumentsBuilder,
            WorkingDirectory = workingDirectory,
            PropertyKeySeparator = ':',
        });

        return result.Success ? result.Output : null;
    }
}
