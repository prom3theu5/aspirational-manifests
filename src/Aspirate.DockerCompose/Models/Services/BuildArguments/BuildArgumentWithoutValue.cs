namespace Aspirate.DockerCompose.Models.Services.BuildArguments;

public class BuildArgumentWithoutValue : IKey, IBuildArgument
{
    public string Key { get; set; } = null!;
}
