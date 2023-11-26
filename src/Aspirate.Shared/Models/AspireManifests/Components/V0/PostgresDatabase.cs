
namespace Aspirate.Shared.Models.AspireManifests.Components.V0;

/// <summary>
/// A Postgres server component for version 0 of Aspire.
/// </summary>
[ExcludeFromCodeCoverage]
public class PostgresDatabase : Resource
{
    /// <summary>
    /// The parent server of the database.
    /// </summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }
}
