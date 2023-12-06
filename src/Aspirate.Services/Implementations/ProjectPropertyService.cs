namespace Aspirate.Services.Implementations;

public sealed class ProjectPropertyService(IFileSystem filesystem, IShellExecutionService shellExecutionService, IAnsiConsole console) : IProjectPropertyService
{
    public async Task<string?> GetProjectPropertiesAsync(string projectPath, params string[] propertyNames)
    {
        var fullProjectPath = filesystem.NormalizePath(projectPath);
        var propertyValues = await ExecuteDotnetMsBuildGetPropertyCommand(fullProjectPath, propertyNames);

        return propertyValues ?? null;
    }

    private async Task<string?> ExecuteDotnetMsBuildGetPropertyCommand(string projectPath, params string[] propertyNames)
    {
        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(DotNetSdkLiterals.MsBuildArgument, string.Empty, quoteValue: false)
            .AppendArgument($"\"{projectPath}\"", string.Empty, quoteValue: false);

        foreach (var propertyName in propertyNames)
        {
            argumentsBuilder.AppendArgument(DotNetSdkLiterals.GetPropertyArgument, propertyName, true);
        }

        var result = await shellExecutionService.ExecuteCommand(new()
        {
            Command = DotNetSdkLiterals.DotNetCommand,
            ArgumentsBuilder = argumentsBuilder,
            PropertyKeySeparator = ':',
        });

        if (!result.Success)
        {
            console.MarkupLine($"[red]Failed to get project properties for '{projectPath}'.[/]");
            throw new ActionCausesExitException(result.ExitCode);
        }

        return result.Success ? result.Output : null;
    }
}
