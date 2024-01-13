namespace Aspirate.Services.Extensions;

public static class DockerfileParametersExtensions
{
    private static readonly StringBuilder _tagBuilder = new();

    public static string ToImageName(this ContainerParameters parameters)
    {
        _tagBuilder.Clear();

        if (!string.IsNullOrEmpty(parameters.Registry))
        {
            _tagBuilder.Append($"{parameters.Registry}/");
        }

        if (!string.IsNullOrEmpty(parameters.Prefix))
        {
            _tagBuilder.Append($"{parameters.Prefix}/");
        }

        _tagBuilder.Append(parameters.ImageName);

        AppendTag(parameters.Tag);

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
