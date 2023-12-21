
namespace Aspirate.Shared.Models.AspireManifests.Components.V0.MongoDb;

[ExcludeFromCodeCoverage]
public class MongoDbDatabaseResource : Resource, IResourceWithParent
{
    /// <summary>
    /// The parent server of the database.
    /// </summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }
}
