using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Amenities;
using Building.Domain.RealEstates;
using Core.Domain.Entities;

namespace Building.Domain.RealEstateAmenities;

[Table("real_estate_amenities", Schema = AssemblyReference.Instance)]
public sealed class RealEstateAmenity : Entity
{
	private RealEstateAmenity() { }
	public RealEstateAmenity(
		Guid realEstateId,
		Guid amenityId) : base() =>
		(RealEstateId, AmenityId) = (realEstateId, amenityId);

	public Guid RealEstateId { get; private set; }
	public Guid AmenityId { get; private set; }
	public RealEstate RealEstate { get; private set; }
	public Amenity Amenity { get; private set; }

	public RealEstateAmenity Update(Guid amenityId)
	{
		AmenityId = amenityId;
		return this;
	}
}
