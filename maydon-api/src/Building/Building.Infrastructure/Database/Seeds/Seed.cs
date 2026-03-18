using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstateTypes;
using Building.Domain.RentalPurposes;
using Common.Domain.Languages;
using Core.Application.Resources;
using Core.Domain.Languages;
using Microsoft.EntityFrameworkCore;

namespace Building.Infrastructure.Database.Seeds;

internal static class Seed
{
	private static IEnumerable<RealEstateType> RealEstateTypes(List<Language> languages)
	{
		if (languages.Any())
		{
			var uzLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.UzbekTwoLetter);
			var ruLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.RussianTwoLetter);
			var enLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.EnglishTwoLetter);

			return [
				new RealEstateType(
					"commercial space",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Biznes markazi"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Бизнес центр"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Business center")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Biznes markazi"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Бизнес центр"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Business center")],
					string.Empty,
					true,
					true,
					true,
					true),
				new RealEstateType(
					"commercial space",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Savdo markazi"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Торговый центр"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Shopping center")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Savdo markazi"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Торговый центр"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Shopping center")],
					string.Empty,
					true,
					true,
					true,
					true),
				new RealEstateType(
					"commercial space",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Maydon"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Помещения"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Retails")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Maydon"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Помещения"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Retails")],
					string.Empty,
					true,
					true,
					false,
					true),
				new RealEstateType(
					"office",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Ofis"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Офис"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Office")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Ofis"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Офис"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Office")],
					string.Empty,
					true,
					true,
					false,
					true),
				new RealEstateType(
					"house",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Hususiy uy"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Частный дом"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"House")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Hususiy uy"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Частный дом"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"House")],
					string.Empty,
					false,
					false,
					true,
					true),
				new RealEstateType(
					"apartment",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Xonadon"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Квартира"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Appartment")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Xonadon"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Квартира"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Appartment")],
					string.Empty,
					false,
					false,
					true,
					true),
				new RealEstateType(
					"land plot",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Yer maydon"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Земельный участок"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Land plot")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Yer maydon"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Земельный участок"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Land plot")],
					string.Empty,
					false,
					false,
					false,
					false),
				new RealEstateType(
					"warehouse",
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Omborxona"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Склад"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Warehouse")],
					[
						new LanguageValue(uzLanguage!.Id,uzLanguage.ShortCode,"Omborxona"),
						new LanguageValue(ruLanguage!.Id,ruLanguage.ShortCode,"Склад"),
						new LanguageValue(enLanguage!.Id,enLanguage.ShortCode,"Warehouse")],
					string.Empty,
					false,
					false,
					false,
					false)];
		}
		return Enumerable.Empty<RealEstateType>();
	}
	private static IEnumerable<RentalPurpose> RentalPurposes(List<Language> languages)
	{
		if (languages.Any())
		{
			var uzLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.UzbekTwoLetter);
			var ruLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.RussianTwoLetter);
			var enLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.EnglishTwoLetter);

			return [
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Ofis"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Офис"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Office")]),
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Ombor"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Склад"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Warehouse")]),
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Chakana savdo"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Розничная торговля"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Retail")]),
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Umumiy ovqatlanish"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Общепит"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Catering")]),
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Ishlab chiqarish"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Производство"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Production")]),
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Yashash"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Жилое"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Residential")]),
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Kovorking"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Коворкинг"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Coworking")]),
				new RentalPurpose([
					new LanguageValue(uzLanguage!.Id, uzLanguage.ShortCode, "Boshqa"),
					new LanguageValue(ruLanguage!.Id, ruLanguage.ShortCode, "Другое"),
					new LanguageValue(enLanguage!.Id, enLanguage.ShortCode, "Other")])];
		}
		return Enumerable.Empty<RentalPurpose>();
	}
	public static async Task SeedingAsync(IServiceProvider serviceProvider, IBuildingDbContext dbContext, CancellationToken cancellationToken)
	{
		var hasNewData = false;

		#region Categories

		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		var realEstateTypes = RealEstateTypes(languages);

		if (realEstateTypes.Any() && !await dbContext.RealEstateTypes.AnyAsync(cancellationToken))
		{
			await dbContext.RealEstateTypes.AddRangeAsync(realEstateTypes, cancellationToken);
			hasNewData = true;
		}

		#endregion

		#region RentalPurposes

		var rentalPurposes = RentalPurposes(languages);

		if (rentalPurposes.Any() && !await dbContext.RentalPurposes.AnyAsync(cancellationToken))
		{
			await dbContext.RentalPurposes.AddRangeAsync(rentalPurposes, cancellationToken);
			hasNewData = true;
		}

		#endregion

		if (hasNewData)
			await dbContext.SaveChangesAsync(cancellationToken);
	}

}

