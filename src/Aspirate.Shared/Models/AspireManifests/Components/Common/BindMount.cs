using System.Runtime.InteropServices;

namespace Aspirate.Shared.Models.AspireManifests.Components.Common;

public class BindMount
{
    private string? _source;

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

    [JsonPropertyName("minikubeMountProcessId")]
    public int? MinikubeMountProcessId { get; set; }

    private static string GetVolumeHostPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "";
        }

        string currentDir = Directory.GetCurrentDirectory();
        string resolvedPath = Path.GetFullPath(Path.Combine(currentDir, path));

        return resolvedPath;
    }
}
