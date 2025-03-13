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
    public bool ReadOnly { get; set; }

    private static string GetVolumeHostPath(string path)
    {
        path = path.Replace('\\', '/').Replace("//", "/");

        string dir = Directory.GetCurrentDirectory();
        string[] subPaths = path.Split("/");

        for (int i = 0; i < subPaths.Length; i++)
        {
            string currentSub = subPaths[i];
            if (currentSub == "..")
            {
                dir = Directory.GetParent(dir).FullName;
            }
            else
            {
                if (dir == Directory.GetDirectoryRoot(dir))
                {
                    dir += currentSub;
                }
                else
                {
                    dir = dir + Path.DirectorySeparatorChar + currentSub;
                }
            }
        }

        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/run/desktop/mnt/host/" + dir.ToLower().Replace(":", "").Replace('\\', '/') : dir;
    }
}

