using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Amenities;
using Building.Domain.Listings;
using Core.Domain.Entities;

namespace Building.Domain.ListingAmenities;

[Table("listing_amenities", Schema = AssemblyReference.Instance)]
public sealed class ListingAmenity : Entity
{
	private ListingAmenity() { }
	public ListingAmenity(
		Guid listingId,
		Guid amenityId) : base() =>
		(ListingId, AmenityId) = (listingId, amenityId);

	public Guid ListingId { get; private set; }
	public Guid AmenityId { get; private set; }
	public Listing Listing { get; private set; }
	public Amenity Amenity { get; private set; }

	public ListingAmenity Update(Guid amenityId)
	{
		AmenityId = amenityId;
		return this;
	}
}
