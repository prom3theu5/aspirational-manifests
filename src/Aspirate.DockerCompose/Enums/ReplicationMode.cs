namespace Aspirate.DockerCompose.Enums;

public enum ReplicationMode
{
    [EnumMember(Value = "replicated")]
    Replicated,

    [EnumMember(Value = "global")]
    Global,
}
