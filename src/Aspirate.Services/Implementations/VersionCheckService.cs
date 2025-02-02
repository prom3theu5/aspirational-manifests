namespace Aspirate.Services.Implementations;

public class VersionCheckService(IFileSystem fs, IAnsiConsole logger) : IVersionCheckService
{
    public Task CheckVersion()
    {
        try
        {
            var aspirateAppDataFolder = fs.AspirateAppDataFolder();
            var updatesDisabledFilePath = fs.Path.Combine(aspirateAppDataFolder, AspirateLiterals.UpdatesDisabledFile);

            if (fs.File.Exists(updatesDisabledFilePath))
            {
                return Task.CompletedTask;
            }

            var lastVersionCheckedFilePath = fs.Path.Combine(aspirateAppDataFolder, AspirateLiterals.LastVersionCheckedFile);

            return fs.File.Exists(lastVersionCheckedFilePath)
                ? HandleLastVersionFile(lastVersionCheckedFilePath)
                : PerformUpdateCheck(lastVersionCheckedFilePath);
        }
        catch (Exception)
        {
            return Task.CompletedTask;
        }
    }

    public async Task SetUpdateChecks(bool isEnabled)
    {
        try
        {
            var aspirateAppDataFolder = fs.AspirateAppDataFolder();
            var updatesDisabledFilePath = fs.Path.Combine(aspirateAppDataFolder, AspirateLiterals.UpdatesDisabledFile);
            var lastVersionCheckedFilePath = fs.Path.Combine(aspirateAppDataFolder, AspirateLiterals.LastVersionCheckedFile);

            if (isEnabled)
            {
                if (fs.File.Exists(updatesDisabledFilePath))
                {
                    fs.File.Delete(updatesDisabledFilePath);
                }

                if (fs.File.Exists(lastVersionCheckedFilePath))
                {
                    fs.File.Delete(lastVersionCheckedFilePath);
                }

                logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Version checks have been [blue]enabled[/].");
                return;
            }

            await File.WriteAllTextAsync(updatesDisabledFilePath, "1");

            logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] Version checks have been [blue]disabled[/].");
        }
        catch (Exception e)
        {
            logger.ValidationFailed(e.Message);
        }
    }

    private Task HandleLastVersionFile(string lastVersionCheckedFilePath)
    {
        try
        {
            var lastCheckedData = fs.File.ReadAllText(lastVersionCheckedFilePath);
            var lastCheckedVersion = JsonSerializer.Deserialize<LastVersionChecked>(lastCheckedData);

            if (lastCheckedVersion is null)
            {
                return PerformUpdateCheck(lastVersionCheckedFilePath);
            }

            if (!NuGetVersion.TryParse(GetCurrentVersion(), out var currentVersion))
            {
                return Task.CompletedTask;
            }

            if (!NuGetVersion.TryParse(lastCheckedVersion.Version, out var lastChecked))
            {
                return Task.CompletedTask;
            }

            if (currentVersion >= lastChecked)
            {
                lastCheckedVersion.Version = currentVersion.ToString();
                lastCheckedVersion.LastChecked = DateTime.UtcNow;

                var updatedLastCheckedData = JsonSerializer.Serialize(lastCheckedVersion);

                return File.WriteAllTextAsync(lastVersionCheckedFilePath, updatedLastCheckedData);
            }

            var lastCheckedDate = lastCheckedVersion.LastChecked;
            var currentDate = DateTime.UtcNow;

            if (currentDate.Subtract(lastCheckedDate).TotalHours >= 1)
            {
                return PerformUpdateCheck(lastVersionCheckedFilePath);
            }

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            logger.ValidationFailed(e.Message);
            return Task.CompletedTask;
        }
    }

    private async Task PerformUpdateCheck(string lastVersionCheckedFilePath)
    {
        try
        {
            if (!NuGetVersion.TryParse(GetCurrentVersion(), out var currentVersion))
            {
                return;
            }

            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<PackageMetadataResource>();

            var metadata = await resource.GetMetadataAsync(
                "aspirate.vnext",
                includePrerelease: true,
                includeUnlisted: false,
                new SourceCacheContext(),
                NullLogger.Instance,
                CancellationToken.None);

            var latestVersion = metadata
                .Where(package => package.IsListed && package.Identity != null).MaxBy(package => package.Identity.Version)
                ?.Identity?.Version;

            var lastChecked = new LastVersionChecked { Version = latestVersion.ToString(), LastChecked = DateTime.UtcNow };

            var lastCheckedData = JsonSerializer.Serialize(lastChecked);

            await File.WriteAllTextAsync(lastVersionCheckedFilePath, lastCheckedData);

            if (latestVersion > currentVersion)
            {
                logger.MarkupLine($"[bold][yellow]A new version of Aspirate is available: [blue]{latestVersion}[/].[/][/]");
                logger.MarkupLine($"[bold][yellow]You are currently using: [blue]{currentVersion}[/].[/][/]");
                logger.MarkupLine(
                    $"[italic][yellow]You can update with: [blue]dotnet tool install -g aspirate --prerelease[/].[/][/]");
            }
        }
        catch (Exception e)
        {
            logger.ValidationFailed(e.Message);
        }
    }

    private static string GetCurrentVersion()
    {
        var attribute = typeof(VersionCheckService).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        return attribute is not null ?
            attribute.InformationalVersion.Split("+").ElementAtOrDefault(0) :
            typeof(VersionCheckService).Assembly.GetName().Version.ToString(3);
    }
}
