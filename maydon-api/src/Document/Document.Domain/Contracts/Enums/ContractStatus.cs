namespace Document.Domain.Contracts.Enums;

public enum ContractStatus
{
    Draft = 0,
    PendingSignature = 1,
    Sent = 2,
    Signed = 3,
    Rejected = 4,
    Archived = 5,
    Failed = 6,
    Cancelled = 7,
    OwnerSigned = 8,
    FullySigned = 9,
    RejectedByOwner = 10,
    RejectedByClient = 11,
    ExpiredUnsigned = 12
}
