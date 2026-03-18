using ContractExternalSyncStatus = Document.Contract.Enums.ExternalSyncStatus;
using DomainExternalSyncStatus = Document.Domain.Shared.ExternalSyncStatus;

namespace Document.Application.Mappings;

/// <summary>
/// Extension methods for mapping Contract (API) enums to Domain enums and vice versa.
/// Contract enums are used in commands/queries/responses (public API).
/// Domain enums are used in entities (persistence layer).
/// </summary>
public static class EnumMappingExtensions
{
    // ──────────────────────────────────────────────
    //  ExternalSyncStatus — same underlying values
    // ──────────────────────────────────────────────

    public static DomainExternalSyncStatus ToDomain(this ContractExternalSyncStatus value) =>
        (DomainExternalSyncStatus)value;

    public static ContractExternalSyncStatus ToContract(this DomainExternalSyncStatus value) =>
        (ContractExternalSyncStatus)value;
}

