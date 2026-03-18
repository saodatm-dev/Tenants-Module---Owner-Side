using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.ProductionTypes.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.ProductionTypes;

[Table("production_types", Schema = AssemblyReference.Instance)]
public sealed class ProductionType : Entity
{
	private ProductionType() { }
	public ProductionType(
		IEnumerable<LanguageValue> translates) : base()
	{
		Raise(new CreateOrUpdateProductionTypeDomainEvent(this.Id, translates));
	}
	public ICollection<ProductionTypeTranslate> Translates { get; private set; }
	public bool IsActive { get; private set; } = true;

	public ProductionType Update(
		IEnumerable<LanguageValue> translates)
	{
		Raise(new CreateOrUpdateProductionTypeDomainEvent(this.Id, translates));
		return this;
	}

	public ProductionType Remove()
	{
		Raise(new RemoveProductionTypeDomainEvent(this.Id));
		return this;
	}

	public ProductionType Activate()
	{
		this.IsActive = true;
		return this;
	}

	public ProductionType Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
