namespace Aspirate.Processors;

public class DockerfileBuilder
{
    private readonly StringBuilder _dockerfileContent = new();

    public DockerfileBuilder From(string image)
    {
        _dockerfileContent.AppendLine($"FROM {image}");
        return this;
    }

    public DockerfileBuilder Copy(string source, string destination)
    {
        _dockerfileContent.AppendLine($"COPY {source} {destination}");
        return this;
    }

    public DockerfileBuilder Run(string command)
    {
        _dockerfileContent.AppendLine($"RUN {command}");
        return this;
    }

    public DockerfileBuilder Cmd(params string[] commands)
    {
        _dockerfileContent.AppendLine($"CMD [\"{string.Join("\", \"", commands)}\"]");
        return this;
    }

    public DockerfileBuilder WorkDir(string directory)
    {
        _dockerfileContent.AppendLine($"WORKDIR {directory}");
        return this;
    }

    public DockerfileBuilder EntryPoint(params string[] commands)
    {
        _dockerfileContent.AppendLine($"ENTRYPOINT [\"{string.Join("\", \"", commands)}\"]");
        return this;
    }

    public string Build() => _dockerfileContent.ToString();
}
