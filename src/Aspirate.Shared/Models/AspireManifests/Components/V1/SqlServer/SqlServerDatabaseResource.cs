namespace Aspirate.Shared.Models.AspireManifests.Components.V1.SqlServer;

[ExcludeFromCodeCoverage]
public class SqlServerDatabaseResource : Resource, IResourceWithParent
{
    /// <summary>
    /// The parent server of the database.
    /// </summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }
}
