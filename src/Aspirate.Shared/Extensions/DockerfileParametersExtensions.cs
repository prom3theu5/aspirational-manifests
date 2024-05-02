namespace Aspirate.Shared.Extensions;

public static class DockerfileParametersExtensions
{
    private static readonly StringBuilder _tagBuilder = new();

    public static string ToImageName(this ContainerOptions options, string resourceKey)
    {
        _tagBuilder.Clear();

        if (!string.IsNullOrEmpty(options.Registry))
        {
            _tagBuilder.Append($"{options.Registry}/");
        }

        if (!string.IsNullOrEmpty(options.Prefix))
        {
            _tagBuilder.Append($"{options.Prefix}/");
        }

        if (string.IsNullOrEmpty(options.ImageName))
        {
            options.ImageName = resourceKey;
        }

        _tagBuilder.Append(options.ImageName);

        AppendTag(options.Tag);

        return _tagBuilder.ToString();
    }

    private static void AppendTag(string? tag)
    {
        if (!string.IsNullOrEmpty(tag))
        {
            _tagBuilder.Append($":{tag}");
            return;
        }

        _tagBuilder.Append(":latest");
    }
}
