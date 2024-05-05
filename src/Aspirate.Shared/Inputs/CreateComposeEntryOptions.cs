namespace Aspirate.Shared.Inputs;

public sealed class CreateComposeEntryOptions : BaseCreateOptions
{
    /// <summary>
    /// Represents the option to include Compose builds in a manifest.
    /// </summary>
    /// <remarks>
    /// Compose builds are used to build Docker images using Docker Compose files.
    /// </remarks>
    public bool? ComposeBuilds { get; set; }
}
