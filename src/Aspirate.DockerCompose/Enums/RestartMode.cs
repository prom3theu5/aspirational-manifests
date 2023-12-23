namespace Aspirate.DockerCompose.Enums;

public enum RestartMode
{
    [EnumMember(Value = "always")]
    Always,

    [EnumMember(Value = "no")]
    No,

    [EnumMember(Value = "on-failure")]
    OnFailure,

    [EnumMember(Value = "unless-stopped")]
    UnlessStopped
}
