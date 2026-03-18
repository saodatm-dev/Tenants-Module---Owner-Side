using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Renovations.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.Renovations;

[Table("renovations", Schema = AssemblyReference.Instance)]
public sealed class Renovation : Entity
{
	private Renovation() { }
	public Renovation(
		IEnumerable<LanguageValue> translates) : base()
	{
		Raise(new CreateOrUpdateRenovationDomainEvent(this.Id, translates));
	}
	public ICollection<RenovationTranslate> Translates { get; private set; }
	public bool IsActive { get; private set; } = true;

	public Renovation Update(
		IEnumerable<LanguageValue> translates)
	{
		Raise(new CreateOrUpdateRenovationDomainEvent(this.Id, translates));
		return this;
	}

	public Renovation Remove()
	{
		Raise(new RemoveRenovationDomainEvent(this.Id));
		return this;
	}

	public Renovation Activate()
	{
		this.IsActive = true;
		return this;
	}

	public Renovation Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
