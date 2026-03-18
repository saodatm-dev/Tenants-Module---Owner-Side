namespace Document.Domain.Shared;

public enum ExternalSyncStatus
{
    Pending = 0,
    Sent = 1,
    Processing = 2,
    Signed = 3,
    Rejected = 4,
    Failed = 5,
    Expired = 6
}
