using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Floors;
using Building.Domain.RealEstates;
using Building.Domain.RoomTypes;
using Core.Domain.Entities;

namespace Building.Domain.Rooms;

[Table("rooms", Schema = AssemblyReference.Instance)]
public sealed class Room : Entity
{
	private Room() { }
	public Room(
		Guid realEstateId,
		Guid roomTypeId,
		Floor? floor,
		string number,
		float? area) : base()
	{
		RealEstateId = realEstateId;
		RoomTypeId = roomTypeId;
		FloorId = floor?.Id;
		Number = number;
		Area = area;
	}
	public Guid RealEstateId { get; private set; }
	public Guid RoomTypeId { get; private set; }
	public Guid? FloorId { get; private set; }
	[Required]
	[MaxLength(50)]
	public string Number { get; private set; }
	public float? Area { get; private set; }
	public RealEstate RealEstate { get; private set; }
	public RoomType RoomType { get; private set; }
	public Floor? Floor { get; private set; }
	public Room Update(
		Guid realEstateId,
		Guid roomTypeId,
		Floor? floor,
		string number,
		float? area)
	{
		RealEstateId = realEstateId;
		RoomTypeId = roomTypeId;
		FloorId = floor?.Id;
		Number = number;
		Area = area;
		return this;
	}
}
