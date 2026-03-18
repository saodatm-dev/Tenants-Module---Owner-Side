namespace Document.Contract.Enums;

/// <summary>
/// External provider synchronization status
/// </summary>
public enum ExternalSyncStatus
{
    /// <summary>
    /// Document is queued for export but not yet sent
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Document successfully sent to external provider
    /// </summary>
    Sent = 1,
    
    /// <summary>
    /// Provider is processing the document
    /// </summary>
    Processing = 2,
    
    /// <summary>
    /// Document signed in external system
    /// </summary>
    Signed = 3,
    
    /// <summary>
    /// Document rejected in external system
    /// </summary>
    Rejected = 4,
    
    /// <summary>
    /// Export or sync failed
    /// </summary>
    Failed = 5,
    
    /// <summary>
    /// Document expired in external system
    /// </summary>
    Expired = 6
}
