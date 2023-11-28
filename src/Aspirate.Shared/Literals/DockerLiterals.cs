namespace Aspirate.Shared.Literals;

public class DockerLiterals
{
    public const string BuildCommand = "build";
    public const string PushCommand = "push";

    // Docker build arguments
    public const string DockerFileArgument = "--file";
    public const string BuildArgArgument = "--build-arg";
    public const string CacheFromArgument = "--cache-from";
    public const string NoCacheArgument = "--no-cache";
    public const string PullArgument = "--pull";
    public const string TagArgument = "--tag";

    // Docker push arguments
    public const string AllTagsArgument = "--all-tags";
}
