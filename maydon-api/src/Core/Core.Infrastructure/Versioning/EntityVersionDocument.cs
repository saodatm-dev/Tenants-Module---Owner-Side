using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Infrastructure.Versioning;

public class EntityVersionDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("entityId")]
    public required string EntityId { get; set; }

    [BsonElement("entityType")]
    public required string EntityType { get; set; }

    [BsonElement("versionNumber")]
    public int VersionNumber { get; set; }

    [BsonElement("data")]
    public BsonDocument? Data { get; set; }

    [BsonElement("changedBy")]
    [BsonIgnoreIfNull]
    public string? ChangedBy { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("changeDescription")]
    [BsonIgnoreIfNull]
    public string? ChangeDescription { get; set; }
}
