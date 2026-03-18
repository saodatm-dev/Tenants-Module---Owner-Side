using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.RealEstates;
using Core.Domain.Entities;

namespace Building.Domain.RealEstateImages;

[Table("real_estate_images", Schema = AssemblyReference.Instance)]
public sealed class RealEstateImage : Entity
{
	private RealEstateImage() { }

	public RealEstateImage(
		Guid realEstateId,
		string objectName,
		bool isPublic = true) : base()
	{
		RealEstateId = realEstateId;
		ObjectName = objectName;
		IsPublic = isPublic;
	}

	public Guid RealEstateId { get; private set; }
	[Required]
	[MaxLength(500)]
	public string ObjectName { get; private set; }
	public bool IsPublic { get; private set; }
	public RealEstate RealEstate { get; private set; }

	public RealEstateImage SetPublic()
	{
		IsPublic = true;
		return this;
	}

	public RealEstateImage SetPrivate()
	{
		IsPublic = false;
		return this;
	}
}
