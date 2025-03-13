namespace Aspirate.Shared.Extensions;

public static class DockerfileParametersExtensions
{
    private static readonly StringBuilder _tagBuilder = new();

    public static List<string> ToImageNames(this ContainerOptions options, string resourceKey)
    {
        var images = new List<string>();

        foreach (var tag in options.Tags)
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

            AppendTag(tag);

            images.Add(_tagBuilder.ToString());
        }

        return images;
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
