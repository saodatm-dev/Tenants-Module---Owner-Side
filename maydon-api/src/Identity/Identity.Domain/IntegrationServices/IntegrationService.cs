using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;

namespace Identity.Domain.IntegrationService;

[Table("integration_services", Schema = AssemblyReference.Instance)]
public sealed class IntegrationService : Entity
{
	private IntegrationService() { }
	public IntegrationService(
		IntegrationServiceType type,
		string value) : base() => (Type, Value) = (type, value);

	public IntegrationServiceType Type { get; private set; }
	[EncryptColumn]
	[Required]
	[MaxLength(1000)]
	public string Value { get; private set; }
}
