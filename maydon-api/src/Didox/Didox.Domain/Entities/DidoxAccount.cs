using Core.Domain.Entities;

namespace Didox.Domain.Entities;

/// <summary>
/// Represent's user's account in Didox
/// </summary>
public sealed class DidoxAccount : ISoftDeleteEntity
{
    /// <summary>
    /// Unique
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Didox login name it should be INN
    /// </summary>
    public required string Login { get; set; }
    
    /// <summary>
    /// Didox Password hashed
    /// </summary>
    public required string Password { get; set; }
    
    /// <summary>
    /// User's unique id in our system
    /// </summary>
    public Guid? OwnerId { get; set; }
    
    /// <summary>
    /// User's Pinfl if he is individual
    /// </summary>
    public string? Pinfl { get; set; }
    
    /// <summary>
    /// User's TIN if he is legal
    /// </summary>
    public string? Tin { get; set; }
    
    /// <summary>
    /// Entity created date and time
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Flag which will define is account deleted or not
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Account Deleted Date
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Who deleted account
    /// </summary>
    public Guid? DeletedBy { get; set; }
    
    /// <summary>
    /// Account updated date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Who updated account
    /// </summary>
    public Guid? UpdatedBy { get; set; }
}
   
