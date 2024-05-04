namespace Aspirate.Shared.Extensions;

public static class PathExtensions
{
    public static string NormalizePath(this IFileSystem fileSystem, string pathToTarget)
    {
        if (string.IsNullOrEmpty(pathToTarget))
        {
            return fileSystem.Directory.GetCurrentDirectory();
        }

        if (!pathToTarget.StartsWith('.'))
        {
            return pathToTarget;
        }

        var currentDirectory = fileSystem.Directory.GetCurrentDirectory();

        var normalizedProjectPath = pathToTarget.Replace('\\', fileSystem.Path.DirectorySeparatorChar);

        return fileSystem.Path.Combine(currentDirectory, normalizedProjectPath);
    }

    public static string GetFullPath(this IFileSystem fileSystem, string path)
    {
        if (fileSystem.Path.IsPathRooted(path))
        {
            return fileSystem.Path.GetFullPath(path);
        }

        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return path.StartsWith($"~{fileSystem.Path.DirectorySeparatorChar}") ?
            // The path is relative to the user's home directory
            fileSystem.Path.Combine(homePath, path.TrimStart('~', fileSystem.Path.DirectorySeparatorChar)) :
            // The path is relative to the current working directory
            fileSystem.Path.GetFullPath(path);
    }

    public static string? GetSecretsStateFilePath(this IFileSystem fileSystem, AspirateState state)
    {
        if (string.IsNullOrEmpty(state.ProjectPath))
        {
            return null;
        }

        var subPath = !string.IsNullOrEmpty(state.OutputPath) ?
            state.OutputPath : (!string.IsNullOrEmpty(state.InputPath) ?
                state.InputPath : AspirateLiterals.DefaultArtifactsPath);

        return fileSystem.Path.Combine(state.ProjectPath, subPath, AspirateLiterals.SecretFileName);
    }
}
