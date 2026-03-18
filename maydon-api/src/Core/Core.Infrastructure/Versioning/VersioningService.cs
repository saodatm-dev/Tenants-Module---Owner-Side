using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Services;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Infrastructure.Versioning;

/// <summary>
/// Service responsible for capturing entity state and publishing version events to Redis Streams.
/// </summary>
public class VersioningService(
    IIntegrationEventPublisher eventPublisher,
    IBackgroundUserContext backgroundUserContext,
    IExecutionContextProvider executionContextProvider) : IVersioningService
{
    private readonly IIntegrationEventPublisher _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    private readonly IBackgroundUserContext _backgroundUserContext = backgroundUserContext ?? throw new ArgumentNullException(nameof(backgroundUserContext));
    private readonly IExecutionContextProvider _executionContextProvider = executionContextProvider ?? throw new ArgumentNullException(nameof(executionContextProvider));

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    private static readonly ConcurrentDictionary<Type, PropertyCache> CachedProperties = new();

    private sealed class PropertyCache
    {
        private readonly Dictionary<string, PropertyInfo> _properties;

        public PropertyCache(Type type)
        {
            _properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p);
        }

        public object? GetValue(object entity, string propertyName)
            => _properties.TryGetValue(propertyName, out var prop) ? prop.GetValue(entity) : null;
    }

    private static PropertyCache GetOrCreateCache(Type type)
        => CachedProperties.GetOrAdd(type, t => new PropertyCache(t));

    public async Task PublishVersionSnapshotAsync<TEntity>(TEntity entity, EntityChangeType changeType, CancellationToken cancellationToken = default)
        where TEntity : IVersionedEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityType = entity.GetType().Name;

        var userId = _backgroundUserContext.UserId ?? (_executionContextProvider.IsAuthorized ? _executionContextProvider.UserId : null);
        var timestamp = DateTime.UtcNow;

        var versionEvent = new EntityVersionChangedEvent
        {
            EntityId = entity.Id,
            EntityType = entityType,
            VersionNumber = entity.CurrentVersion,
            Data = GetEntityAsString(entity),
            ChangedBy = userId,
            Timestamp = timestamp,
            ChangeType = changeType
        };

        await _eventPublisher.PublishAsync(versionEvent, cancellationToken);
    }

    private string GetEntityAsString<TEntity>(TEntity entity) where TEntity : IVersionedEntity
    {
        try
        {
            var entityType = entity.GetType();
            var typeName = entityType.Name;

            if (typeName.Contains("Contract", StringComparison.Ordinal))
            {
                return SerializeContract(entity, entityType);
            }

            return JsonSerializer.Serialize(entity, entityType, JsonOptions);
        }
        catch (JsonException ex)
        {
            return JsonSerializer.Serialize(new
            {
                entity.Id,
                EntityType = entity.GetType().Name,
                SerializationError = ex.Message,
                Timestamp = DateTime.UtcNow
            }, JsonOptions);
        }
    }

    private static string SerializeContract<TEntity>(TEntity entity, Type entityType) where TEntity : IVersionedEntity
    {
        var cache = GetOrCreateCache(entityType);

        var versionDto = new
        {
            entity.Id,
            TenantId = cache.GetValue(entity, "TenantId"),
            TemplateId = cache.GetValue(entity, "TemplateId"),
            ContractNumber = cache.GetValue(entity, "ContractNumber"),
            Status = cache.GetValue(entity, "Status"),
            Language = cache.GetValue(entity, "Language"),
            Body = cache.GetValue(entity, "Body"),
            LeaseId = cache.GetValue(entity, "LeaseId"),
            RealEstateId = cache.GetValue(entity, "RealEstateId"),
            OwnerCompanyId = cache.GetValue(entity, "OwnerCompanyId"),
            ClientCompanyId = cache.GetValue(entity, "ClientCompanyId"),
            OwnerInn = cache.GetValue(entity, "OwnerInn"),
            ClientInn = cache.GetValue(entity, "ClientInn"),
            MonthlyAmount = cache.GetValue(entity, "MonthlyAmount"),
            LeaseStartDate = cache.GetValue(entity, "LeaseStartDate"),
            LeaseEndDate = cache.GetValue(entity, "LeaseEndDate"),
            RejectionReason = cache.GetValue(entity, "RejectionReason"),
            SignatureDeadline = cache.GetValue(entity, "SignatureDeadline"),
            entity.CurrentVersion,
            IntegrationStates = cache.GetValue(entity, "IntegrationStates"),
            IsDeleted = cache.GetValue(entity, "IsDeleted"),
            DeletedAt = cache.GetValue(entity, "DeletedAt"),
            DeletedBy = cache.GetValue(entity, "DeletedBy")
        };

        return JsonSerializer.Serialize(versionDto, JsonOptions);
    }

    public async Task PublishDeletionSnapshotAsync(Guid entityId, string entityType, int versionNumber, CancellationToken cancellationToken = default)
    {
        var userId = _backgroundUserContext.UserId ?? (_executionContextProvider.IsAuthorized ? _executionContextProvider.UserId : null);
        var timestamp = DateTime.UtcNow;

        var versionEvent = new EntityVersionChangedEvent
        {
            EntityId = entityId,
            EntityType = entityType,
            VersionNumber = versionNumber,
            Data = "{}",
            ChangedBy = userId,
            Timestamp = timestamp,
            ChangeType = EntityChangeType.Deleted
        };

        await _eventPublisher.PublishAsync(versionEvent, cancellationToken);
    }
}
