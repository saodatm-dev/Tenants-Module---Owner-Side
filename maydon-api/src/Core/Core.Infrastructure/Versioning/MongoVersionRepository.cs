using Core.Application.Abstractions.Repositories;
using Core.Application.Pagination;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Versioning;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Infrastructure.Versioning;

/// <summary>
/// MongoDB implementation of a version repository for cold storage.
/// Stores complete version history with unlimited retention.
/// </summary>
public sealed class MongoVersionRepository : IVersionRepository
{
    private readonly IMongoCollection<EntityVersionDocument> _versionsCollection;
    private readonly ILogger<MongoVersionRepository> _logger;

    public MongoVersionRepository(IMongoDatabase database, ILogger<MongoVersionRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(database);
        _logger = logger;
        _versionsCollection = database.GetCollection<EntityVersionDocument>("entity_versions");

        CreateIndexes();

        if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(EntityVersionDocument)))
        {
            MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<EntityVersionDocument>(cm =>
            {
                cm.AutoMap();
                cm.SetDiscriminatorIsRequired(false);
            });
        }
    }

    public async Task SaveVersionAsync(EntityVersionChangedEvent @event, CancellationToken cancellationToken = default)
    {
        var jsonString = @event.Data;
        var dataBson = BsonDocument.Parse(jsonString);

        var filter = Builders<EntityVersionDocument>.Filter.And(
            Builders<EntityVersionDocument>.Filter.Eq(x => x.EntityId, @event.EntityId.ToString()),
            Builders<EntityVersionDocument>.Filter.Eq(x => x.VersionNumber, @event.VersionNumber)
        );

        // IDEMPOTENCY: Check if document exists to preserve ObjectId
        var existingDoc = await _versionsCollection
            .Find(filter)
            .Limit(1)
            .FirstOrDefaultAsync(cancellationToken);

        var document = new EntityVersionDocument
        {
            Id = existingDoc?.Id ?? ObjectId.GenerateNewId(),
            EntityId = @event.EntityId.ToString(),
            EntityType = @event.EntityType,
            VersionNumber = @event.VersionNumber,
            Data = dataBson,
            ChangedBy = @event.ChangedBy?.ToString(),
            Timestamp = @event.Timestamp,
            ChangeDescription = @event.ChangeDescription ?? @event.ChangeType.ToString()
        };

        var options = new ReplaceOptions { IsUpsert = true };
        await _versionsCollection.ReplaceOneAsync(filter, document, options, cancellationToken);
    }

    public async Task<PagedList<VersionMetadata>> GetHistoryAsync(
        Guid entityId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var filter = Builders<EntityVersionDocument>.Filter.Eq(x => x.EntityId, entityId.ToString());
        var sort = Builders<EntityVersionDocument>.Sort.Descending(x => x.VersionNumber);

        var projection = Builders<EntityVersionDocument>.Projection
            .Include(x => x.VersionNumber)
            .Include(x => x.Timestamp)
            .Include(x => x.ChangedBy)
            .Include(x => x.ChangeDescription)
            .Include(x => x.Data);

        var totalCount = await _versionsCollection.CountDocumentsAsync(
            filter, cancellationToken: cancellationToken);

        var documents = await _versionsCollection
            .Find(filter)
            .Project<EntityVersionDocument>(projection)
            .Sort(sort)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var items = documents.Select(doc => new VersionMetadata
        {
            VersionNumber = doc.VersionNumber,
            CreatedAt = doc.Timestamp,
            CreatedBy = !string.IsNullOrEmpty(doc.ChangedBy)
                ? Guid.Parse(doc.ChangedBy)
                : null,
            ChangeDescription = doc.ChangeDescription,
            DataSize = doc.Data?.ToBson().Length ?? 0
        }).ToList();

        return new PagedList<VersionMetadata>(
            items,
            pageNumber,
            pageSize,
            (int)totalCount);
    }

    public async Task<string?> GetVersionDataAsync(
        Guid entityId, int versionNumber, CancellationToken cancellationToken = default)
    {
        var filter = Builders<EntityVersionDocument>.Filter.And(
            Builders<EntityVersionDocument>.Filter.Eq(x => x.EntityId, entityId.ToString()),
            Builders<EntityVersionDocument>.Filter.Eq(x => x.VersionNumber, versionNumber)
        );

        var document = await _versionsCollection
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        if (document?.Data == null)
            return null;

        return document.Data.ToJson(new MongoDB.Bson.IO.JsonWriterSettings
        {
            OutputMode = MongoDB.Bson.IO.JsonOutputMode.RelaxedExtendedJson
        });
    }

    private void CreateIndexes()
    {
        try
        {
            var existingIndexes = _versionsCollection.Indexes.List().ToList();
            var existingIndexNames = existingIndexes
                .SelectMany(idx => idx.Elements)
                .Where(e => e.Name == "name")
                .Select(e => e.Value.AsString)
                .ToHashSet();

            if (!existingIndexNames.Contains("idx_entityId_versionNumber"))
            {
                var entityIdVersionIndex = Builders<EntityVersionDocument>.IndexKeys
                    .Ascending(x => x.EntityId)
                    .Descending(x => x.VersionNumber);

                _versionsCollection.Indexes.CreateOne(
                    new CreateIndexModel<EntityVersionDocument>(
                        entityIdVersionIndex,
                        new CreateIndexOptions { Name = "idx_entityId_versionNumber" }));
            }

            if (!existingIndexNames.Contains("idx_timestamp"))
            {
                var timestampIndex = Builders<EntityVersionDocument>.IndexKeys
                    .Descending(x => x.Timestamp);

                _versionsCollection.Indexes.CreateOne(
                    new CreateIndexModel<EntityVersionDocument>(
                        timestampIndex,
                        new CreateIndexOptions { Name = "idx_timestamp" }));
            }
        }
        catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict")
        {
            // Index already exists with different options — acceptable
        }
        catch (MongoCommandException ex)
        {
            _logger.LogWarning(ex,
                "MongoDB index creation skipped — command {Code} failed. Ensure the connection string includes valid credentials. Indexes will be created on next successful startup",
                ex.CodeName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "MongoDB index creation skipped due to unexpected error. Repository will operate without custom indexes until next restart");
        }
    }
}
