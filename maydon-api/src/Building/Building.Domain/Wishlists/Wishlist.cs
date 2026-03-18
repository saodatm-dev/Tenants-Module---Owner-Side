using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Wishlists;

[Table("wishlists", Schema = AssemblyReference.Instance)]
public sealed class Wishlist : Entity
{
	private Wishlist() { }
	public Wishlist(
		Guid tenantId,
		Guid userId,
		Guid listingId) : base() // might be complex, building, real estate , listing etc.
	{
		TenantId = tenantId;
		UserId = userId;
		ListringId = listingId;
	}
	public Guid TenantId { get; private set; }
	public Guid UserId { get; private set; }
	public Guid ListringId { get; private set; }
	public Wishlist Update(Guid listingId)
	{
		ListringId = listingId;
		return this;
	}
	public Wishlist Restore()
	{
		this.IsDeleted = false;
		return this;
	}
}
