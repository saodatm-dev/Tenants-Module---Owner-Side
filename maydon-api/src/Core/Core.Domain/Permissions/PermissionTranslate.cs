using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Core.Domain.Permissions;

[Table("permission_translates")]
public sealed class PermissionTranslate : Entity
{
	private PermissionTranslate() { }
	public PermissionTranslate(
		Guid permissionId,
		string languageCode,
		string value) : base()
	{
		this.PermissionId = permissionId;
		this.LanguageShortCode = languageCode;
		this.Value = value;
	}

	public Guid PermissionId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public Permission Permission { get; private set; }
	public PermissionTranslate Update(
		Guid permissionId,
		string languageCode,
		string value)
	{
		this.PermissionId = permissionId;
		this.LanguageShortCode = languageCode;
		this.Value = value;
		return this;
	}
}
