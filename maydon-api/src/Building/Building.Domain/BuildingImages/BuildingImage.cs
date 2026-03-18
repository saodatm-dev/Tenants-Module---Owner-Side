using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.BuildingImages;

[Table("building_images", Schema = AssemblyReference.Instance)]
public sealed class BuildingImage : Entity
{
	private BuildingImage() { }

	public BuildingImage(
		Guid buildingId,
		string objectName,
		bool isPublic = true) : base()
	{
		this.BuildingId = buildingId;
		this.ObjectName = objectName;
		this.IsPublic = isPublic;
	}

	public Guid BuildingId { get; private set; }
	[Required]
	[MaxLength(500)]
	public string ObjectName { get; private set; }
	public Building.Domain.Buildings.Building Building { get; private set; }
	public bool IsPublic { get; private set; }

	public BuildingImage SetPublic()
	{
		this.IsPublic = true;
		return this;
	}
	public BuildingImage SetPrivate()
	{
		this.IsPublic = false;
		return this;
	}
	public BuildingImage Update(string objectName)
	{
		this.ObjectName = objectName;
		return this;
	}
}
