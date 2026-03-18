using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;

namespace Core.Domain.Permissions;

[Table("permissions")]
public sealed class Permission : Entity
{
	private Permission() { }
	public Permission(
		string instance,
		string group,
		string name,
		bool isSystem = false) : base()
	{
		Instance = instance;
		Group = group;
		Name = name;
		IsSystem = isSystem;
	}

	[Required]
	[MaxLength(100)]
	public string Instance { get; private set; }
	[Required]
	[MaxLength(100)]
	public string Group { get; private set; }
	[EncryptColumn]
	[Required]
	[MaxLength(200)]
	public string Name { get; private set; }
	public bool IsSystem { get; private set; }
	public bool IsActive { get; private set; } = true;
	public ICollection<PermissionTranslate> Translates { get; private set; }

	public Permission Update(
		string group,
		string name,
		bool isSystem = false)
	{
		Group = group;
		Name = name;
		IsSystem = isSystem;
		return this;
	}
	public Permission Activate()
	{
		IsActive = true;
		return this;
	}
	public Permission Deactivate()
	{
		IsActive = false;
		return this;
	}
}
