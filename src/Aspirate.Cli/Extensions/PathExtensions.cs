namespace Aspirate.Cli.Extensions;

public static class PathExtensions
{
    public static string NormalizePath(this IFileSystem fileSystem, string pathToTarget)
    {
        if (!pathToTarget.StartsWith('.'))
        {
            return pathToTarget;
        }

        var currentDirectory = fileSystem.Directory.GetCurrentDirectory();

        var normalizedProjectPath = pathToTarget.Replace('\\', fileSystem.Path.DirectorySeparatorChar);

        return fileSystem.Path.Combine(currentDirectory, normalizedProjectPath);
    }
}
