using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.RoomTypes;

[Table("room_type_translates", Schema = AssemblyReference.Instance)]
public sealed class RoomTypeTranslate : Entity
{
	private RoomTypeTranslate() { }
	public RoomTypeTranslate(
		Guid roomTypeId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		RoomTypeId = roomTypeId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}
	public Guid RoomTypeId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(100)]
	public string Value { get; private set; }
	public RoomType RoomType { get; private set; }

	public RoomTypeTranslate Update(
		Guid languageId,
		string languageShortCode,
		string value)
	{
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
		return this;
	}
}
