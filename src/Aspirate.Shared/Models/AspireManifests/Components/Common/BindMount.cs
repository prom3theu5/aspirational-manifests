namespace Aspirate.Shared.Models.AspireManifests.Components.Common;

public class BindMount
{
    public string? MinikubeHostPathPrefix { get; set; } = "/mount";

    private string? _source;

    [JsonPropertyName("source")]
    public string? Source
    {
        get => _source;
        set => _source = GetSource(value);
    }

    [JsonPropertyName("target")]
    public string? Target { get; set; }

    [JsonPropertyName("readOnly")]
    public bool? ReadOnly { get; set; }

    private static string GetSource(string path)
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
