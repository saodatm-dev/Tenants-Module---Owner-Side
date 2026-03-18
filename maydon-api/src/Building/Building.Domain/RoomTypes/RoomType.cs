using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.RoomTypes.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.RoomTypes;

[Table("room_types", Schema = AssemblyReference.Instance)]
public sealed class RoomType : Entity
{
	private RoomType() { }
	public RoomType(
		IEnumerable<LanguageValue> translates) : base()
	{
		Raise(new CreateOrUpdateRoomTypeDomainEvent(this.Id, translates));
	}
	public ICollection<RoomTypeTranslate> Translates { get; private set; }
	public RoomType Update(
		IEnumerable<LanguageValue> translates)
	{
		Raise(new CreateOrUpdateRoomTypeDomainEvent(this.Id, translates));
		return this;
	}
	// to remove regions translates
	public RoomType Remove()
	{
		Raise(new RemoveRoomTypeDomainEvent(this.Id));
		return this;
	}
}
