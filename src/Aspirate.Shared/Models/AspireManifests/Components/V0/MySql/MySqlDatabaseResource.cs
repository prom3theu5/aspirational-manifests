
namespace Aspirate.Shared.Models.AspireManifests.Components.V0.MySql;

[ExcludeFromCodeCoverage]
public class MySqlDatabaseResource : Resource, IResourceWithParent
{
    /// <summary>
    /// The parent server of the database.
    /// </summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }
}
