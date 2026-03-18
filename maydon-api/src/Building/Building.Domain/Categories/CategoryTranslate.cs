using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Categories;

[Table("category_translates", Schema = AssemblyReference.Instance)]
public sealed class CategoryTranslate : Entity
{
	private CategoryTranslate() { }
	public CategoryTranslate(
		Guid categoryId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		CategoryId = categoryId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid CategoryId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public Category Category { get; private set; }

	public CategoryTranslate Update(
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
