using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.RentalPurposes.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.RentalPurposes;

[Table("rental_purposes", Schema = AssemblyReference.Instance)]
public sealed class RentalPurpose : Entity
{
	private RentalPurpose() { }
	public RentalPurpose(
		IEnumerable<LanguageValue> translates) : base()
	{
		Raise(new UpsertRentalPurposeDomainEvent(this.Id, translates));
	}
	public ICollection<RentalPurposeTranslate> Translates { get; private set; }
	public bool IsActive { get; private set; } = true;

	public RentalPurpose Update(
		IEnumerable<LanguageValue> translates)
	{
		Raise(new UpsertRentalPurposeDomainEvent(this.Id, translates));
		return this;
	}

	public RentalPurpose Remove()
	{
		Raise(new RemoveRentalPurposeDomainEvent(this.Id));
		return this;
	}

	public RentalPurpose Activate()
	{
		this.IsActive = true;
		return this;
	}

	public RentalPurpose Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
