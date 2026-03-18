using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Common.Domain.Languages;

[Table("languages", Schema = AssemblyReference.Instance)]
public sealed class Language : Entity
{
	private Language() { }
	public Language(
		string name,
		string shortCode) : base()
	{
		this.Name = name;
		this.ShortCode = shortCode;
	}
    
	[MaxLength(100)]
	public string Name { get; private set; }
	[MaxLength(10)]
	public string ShortCode { get; private set; }
	public int Order { get; private set; }
	public bool IsActive { get; private set; } = true;
	public Language Update(
		string name,
		string shortCode)
	{
		this.Name = name.Trim();
		this.ShortCode = shortCode.Trim();
		return this;
	}
	public Language Activate()
	{
		this.IsActive = true;
		return this;
	}
	public Language Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
