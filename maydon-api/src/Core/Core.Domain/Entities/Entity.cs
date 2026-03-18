using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Core.Domain.Events;

namespace Core.Domain.Entities;

public abstract class Entity : IModifiableEntity, IAuditableEntity, ISoftDeleteEntity, IHasDomainEvents
{
	private readonly List<IDomainEvent> domainEvents = [];
	[NotMapped]
	[JsonIgnore]
	public List<IDomainEvent> DomainEvents => [.. domainEvents];

	public void ClearDomainEvents() =>
		domainEvents.Clear();

	public void Raise(IDomainEvent domainEvent) =>
		domainEvents.Add(domainEvent);

	protected Entity() : this(Guid.CreateVersion7()) { }
	protected Entity(Guid id) => this.Id = id;
	[Key]
	public Guid Id { get; private set; }
	public Guid? CreatedBy { get; set; }
	public Guid? UpdatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public DateTime? DeletedAt { get; set; }
	public bool IsDeleted { get; set; }
	public Guid? DeletedBy { get; set; }
}

