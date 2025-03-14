using System.Runtime.InteropServices;

namespace Aspirate.Shared.Models.AspireManifests.Components.Common;

public class BindMount
{
    private string? _source;
    //private const string WindowsDockerHostPath = "/run/desktop/mnt/host/";

    [JsonPropertyName("source")]
    public string? Source
    {
        get => _source;
        set => _source = GetVolumeHostPath(value);
    }

    [JsonPropertyName("target")]
    public string? Target { get; set; }

    [JsonPropertyName("readOnly")]
    public bool? ReadOnly { get; set; }

    private static string GetVolumeHostPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "";
        }

        string currentDir = Directory.GetCurrentDirectory();
        string resolvedPath = Path.GetFullPath(Path.Combine(currentDir, path));

        return resolvedPath;

        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //{
        //    // Normalize for Docker Desktop on Windows (Minikube / Docker Desktop for Windows)
        //    string mountPath = resolvedPath.ToLower()
        //                                    .Replace(":", "")  // Remove drive letter colon
        //                                    .Replace('\\', '/'); // Convert Windows-style slashes

        //    return mountPath;
        //}
        //else
        //{
        //    return resolvedPath; // Linux paths are already in the correct format
        //}
    }
}
