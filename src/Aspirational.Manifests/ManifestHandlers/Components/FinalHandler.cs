namespace Aspirational.Manifests.ManifestHandlers.Components;

/// <summary>
/// A project component for version 0 of Aspire.
/// </summary>
public class FinalHandler : BaseHandler
{
    /// <inheritdoc />
    public override string ResourceType => AspireResourceLiterals.Final;

    /// <inheritdoc />
    public override Resource Deserialize(ref Utf8JsonReader reader) =>
        throw new NotImplementedException();
}
