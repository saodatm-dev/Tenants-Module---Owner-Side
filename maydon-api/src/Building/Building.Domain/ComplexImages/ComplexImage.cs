using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Complexes;
using Core.Domain.Entities;

namespace Building.Domain.ComplexImages;

[Table("complex_images", Schema = AssemblyReference.Instance)]
public sealed class ComplexImage : Entity
{
	private ComplexImage() { }

	public ComplexImage(
		Guid complexId,
		string objectName,
		bool isPublic = true) : base()
	{
		this.ComplexId = complexId;
		this.ObjectName = objectName;
		this.IsPublic = isPublic;
	}

	public Guid ComplexId { get; private set; }
	[Required]
	[MaxLength(500)]
	public string ObjectName { get; private set; }
	public bool IsPublic { get; private set; }
	public Complex Complex { get; private set; }

	public ComplexImage SetPublic()
	{
		this.IsPublic = true;
		return this;
	}
	public ComplexImage SetPrivate()
	{
		this.IsPublic = false;
		return this;
	}
	public ComplexImage Update(string objectName)
	{
		this.ObjectName = objectName;
		return this;
	}
}
