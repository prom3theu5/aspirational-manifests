namespace Aspirate.Services.Implementations;

public class AspireManifestCompositionService(IFileSystem fileSystem, IAnsiConsole console, IShellExecutionService shellExecutionService) : IAspireManifestCompositionService
{
    public async Task<(bool Success, string FullPath)> BuildManifestForProject(string appHostProject, string? launchProfile)
    {
        var normalizedPath = fileSystem.NormalizePath(appHostProject);

        var argumentsBuilder = ArgumentsBuilder.Create()
            .AppendArgument(DotNetSdkLiterals.RunArgument, string.Empty, quoteValue: false);

        if (!string.IsNullOrEmpty(launchProfile))
        {
            console.MarkupLine($"[bold]Using Custom Launch Profile: [blue]{launchProfile}[/][/]");

            argumentsBuilder
                .AppendArgument(DotNetSdkLiterals.LaunchProfileArgument, launchProfile, quoteValue: false);
        }

        argumentsBuilder
            .AppendArgument(DotNetSdkLiterals.ProjectArgument, normalizedPath)
            .AppendArgument(DotNetSdkLiterals.ArgumentDelimiter, string.Empty, quoteValue: false)
            .AppendArgument(DotNetSdkLiterals.PublisherArgument, AspireLiterals.ManifestPublisherArgument, quoteValue: false)
            .AppendArgument(DotNetSdkLiterals.OutputPathArgument, AspireLiterals.DefaultManifestFile, quoteValue: false);

        var appHostDirectory = fileSystem.Path.GetDirectoryName(normalizedPath);
        var outputFile = fileSystem.Path.Combine(appHostDirectory, AspireLiterals.DefaultManifestFile);

        if (fileSystem.File.Exists(outputFile))
        {
            fileSystem.File.Delete(outputFile);
        }

        var newManifestFile = await shellExecutionService.ExecuteCommand(new()
        {
            Command = DotNetSdkLiterals.DotNetCommand,
            PreCommandMessage = "Generating Aspire Manifest...",
            ArgumentsBuilder =  argumentsBuilder,
            ShowOutput = false,
        });

        if (!string.IsNullOrEmpty(newManifestFile.Error))
        {
            console.MarkupLine($"[red]Error: {newManifestFile.Error}[/]");
            console.MarkupLine($"[red]Could not build the manifest for the supplied details. Exiting rather than building default manifest file.[/]");
            ActionCausesExitException.ExitNow();
        }

        return (newManifestFile.Success, outputFile);
    }
}
